import type { FileMetadata } from "@/models/SecureFileDto";

export default class AuthenticatedSecretKeyCryptographyService {
  public static readonly KEY_LENGTH_IN_BYTES = 32;
  public static readonly SALT_LENGTH_IN_BYTES = 16;
  public static readonly TAG_LENGTH_IN_BYTES = 16;

  private readonly NONCE_LENGTH = 12;
  private readonly tagLengthInBytes: number;
  private readonly ALGORITHM = "AES-GCM";

  private readonly masterKey: string | Uint8Array;
  private readonly salt: Uint8Array<ArrayBuffer>;
  private readonly requirePassword: boolean;

  private derivedKey: ArrayBuffer | null = null;
  private cryptoKey: CryptoKey | null = null;
  private nonceBase: ArrayBuffer | null = null;
  private metadataKey: CryptoKey | null = null;

  public seq: number;

  constructor(
    password?: string,
    b64Key?: string,
    tagLengthInBytes = AuthenticatedSecretKeyCryptographyService.TAG_LENGTH_IN_BYTES
  ) {
    this.tagLengthInBytes = tagLengthInBytes;
    this.seq = 0;

    if (b64Key) {
      const arrayKey = this.base64ToArray(b64Key);
      if (!password) {
        this.salt = arrayKey.slice(
          0,
          AuthenticatedSecretKeyCryptographyService.SALT_LENGTH_IN_BYTES
        ) as Uint8Array<ArrayBuffer>;
        this.masterKey = arrayKey.slice(
          AuthenticatedSecretKeyCryptographyService.SALT_LENGTH_IN_BYTES
        );
      } else {
        this.salt = arrayKey as Uint8Array<ArrayBuffer>;
        this.masterKey = password;
      }
    } else {
      this.salt = crypto.getRandomValues(
        new Uint8Array(
          AuthenticatedSecretKeyCryptographyService.SALT_LENGTH_IN_BYTES
        )
      );
      this.masterKey = password
        ? password
        : crypto.getRandomValues(
            new Uint8Array(
              AuthenticatedSecretKeyCryptographyService.KEY_LENGTH_IN_BYTES
            )
          );
    }
    this.requirePassword = password ? true : false;
  }

  private base64ToArray(b64Key: string): Uint8Array {
    return new Uint8Array(
      atob(b64Key)
        .split("")
        .map((c) => c.charCodeAt(0))
    );
  }

  async start(): Promise<void> {
    this.derivedKey = this.requirePassword
      ? await this.derivePbkdfKeyMaterial(this.masterKey as string)
      : await this.deriveHkdfKeyMaterial(this.masterKey as Uint8Array);

    this.cryptoKey = await crypto.subtle.importKey(
      "raw",
      this.derivedKey!,
      { name: this.ALGORITHM },
      true,
      ["encrypt", "decrypt"]
    );

    this.nonceBase = await this.generateNonceBase(this.derivedKey!);
    this.metadataKey = await this.deriveMetadataKey(this.derivedKey!);
  }

  private async generateNonceBase(
    derivedKey: ArrayBuffer
  ): Promise<ArrayBuffer> {
    const encoder = new TextEncoder();
    const inputKey = await crypto.subtle.importKey(
      "raw",
      derivedKey,
      "HKDF",
      false,
      ["deriveKey"]
    );

    const base = await crypto.subtle.exportKey(
      "raw",
      await crypto.subtle.deriveKey(
        {
          name: "HKDF",
          salt: this.salt,
          info: encoder.encode("Content-Encoding: nonce\0"),
          hash: "SHA-256",
        },
        inputKey,
        {
          name: "AES-GCM",
          length: 128,
        },
        true,
        ["encrypt", "decrypt"]
      )
    );

    return base.slice(0, this.NONCE_LENGTH);
  }

  private generateNonce(seq: number, nonceBase: ArrayBuffer) {
    if (seq > 0xffffffff) {
      throw new Error("record sequence number exceeds limit");
    }
    const nonce = new DataView(nonceBase.slice(0));
    const m = nonce.getUint32(nonce.byteLength - 4);
    const xor = (m ^ seq) >>> 0; //forces unsigned int xor
    nonce.setUint32(nonce.byteLength - 4, xor);
    return new Uint8Array(nonce.buffer);
  }

  private async encryptWithKey(
    data: Uint8Array,
    seq: number,
    cryptoKey: CryptoKey,
    nonceBase: ArrayBuffer
  ): Promise<ArrayBuffer> {
    const nonce = this.generateNonce(seq, nonceBase);
    return await crypto.subtle.encrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      cryptoKey,
      data.buffer as ArrayBuffer
    );
  }

  public async encrypt(data: Uint8Array, seq: number): Promise<ArrayBuffer> {
    if (!this.cryptoKey || !this.nonceBase) {
      throw new Error("Service not initialized. Call start() first.");
    }
    return this.encryptWithKey(data, seq, this.cryptoKey!, this.nonceBase!);
  }

  private async decryptWithKey(
    data: Uint8Array,
    seq: number,
    cryptoKey: CryptoKey,
    nonceBase: ArrayBuffer
  ): Promise<ArrayBuffer> {
    const nonce = this.generateNonce(seq, nonceBase);
    return await crypto.subtle.decrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      cryptoKey,
      data as Uint8Array<ArrayBuffer>
    );
  }

  public async decrypt(data: Uint8Array, seq: number): Promise<ArrayBuffer> {
    if (!this.cryptoKey || !this.nonceBase) {
      throw new Error("Service not initialized. Call start() first.");
    }
    return this.decryptWithKey(data, seq, this.cryptoKey!, this.nonceBase!);
  }

  private async derivePbkdfKeyMaterial(password: string): Promise<ArrayBuffer> {
    const encoder = new TextEncoder();
    const keyMaterial = await crypto.subtle.importKey(
      "raw",
      encoder.encode(password),
      "PBKDF2",
      false,
      ["deriveBits"]
    );
    return await crypto.subtle.deriveBits(
      { name: "PBKDF2", salt: this.salt, iterations: 1e6, hash: "SHA-256" },
      keyMaterial,
      256
    );
  }

  private async deriveHkdfKeyMaterial(
    masterKey: Uint8Array
  ): Promise<ArrayBuffer> {
    const keyMaterial = await crypto.subtle.importKey(
      "raw",
      masterKey as BufferSource,
      "HKDF",
      false,
      ["deriveBits"]
    );
    const info = new TextEncoder().encode("info");
    return await crypto.subtle.deriveBits(
      {
        name: "HKDF",
        salt: this.salt,
        info,
        hash: "SHA-256",
      },
      keyMaterial,
      256
    );
  }

  getSecret(): string {
    if (this.requirePassword) return btoa(String.fromCharCode(...this.salt));
    return btoa(
      String.fromCharCode(
        ...new Uint8Array([
          ...this.salt,
          ...new Uint8Array(this.masterKey as Uint8Array),
        ])
      )
    );
  }

  private async deriveMetadataKey(derivedKey: ArrayBuffer): Promise<CryptoKey> {
    const encoder = new TextEncoder();
    const inputKey = await crypto.subtle.importKey(
      "raw",
      derivedKey,
      "HKDF",
      false,
      ["deriveKey"]
    );

    return await crypto.subtle.deriveKey(
      {
        name: "HKDF",
        salt: this.salt,
        info: encoder.encode("metadata-encryption-v1"),
        hash: "SHA-256",
      },
      inputKey,
      {
        name: "AES-GCM",
        length: 256,
      },
      false,
      ["encrypt", "decrypt"]
    );
  }

  private async encryptMetadataWithKey(
    metadata: object,
    metadataKey: CryptoKey
  ): Promise<string> {
    const encoder = new TextEncoder();
    const jsonData = encoder.encode(JSON.stringify(metadata));
    const nonce = crypto.getRandomValues(new Uint8Array(this.NONCE_LENGTH));

    const encryptedData = await crypto.subtle.encrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      metadataKey,
      jsonData
    );

    const combined = new Uint8Array(nonce.length + encryptedData.byteLength);
    combined.set(nonce, 0);
    combined.set(new Uint8Array(encryptedData), nonce.length);

    return btoa(String.fromCharCode(...combined));
  }

  public async encryptMetadata(metadata: object): Promise<string> {
    if (!this.metadataKey) {
      throw new Error("Service not initialized. Call start() first.");
    }
    return this.encryptMetadataWithKey(metadata, this.metadataKey!);
  }
  private async decryptMetadataWithKey(
    encryptedBase64: string,
    metadataKey: CryptoKey
  ): Promise<FileMetadata> {
    const combined = new Uint8Array(
      atob(encryptedBase64)
        .split("")
        .map((c) => c.charCodeAt(0))
    );

    const nonce = combined.slice(0, this.NONCE_LENGTH);
    const ciphertext = combined.slice(this.NONCE_LENGTH);

    const decryptedData = await crypto.subtle.decrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      metadataKey,
      ciphertext
    );

    const decoder = new TextDecoder();
    const jsonString = decoder.decode(decryptedData);
    return JSON.parse(jsonString);
  }

  public async decryptMetadata(encryptedBase64: string): Promise<FileMetadata> {
    if (!this.metadataKey) {
      throw new Error("Service not initialized. Call start() first.");
    }
    return this.decryptMetadataWithKey(encryptedBase64, this.metadataKey!);
  }
}
