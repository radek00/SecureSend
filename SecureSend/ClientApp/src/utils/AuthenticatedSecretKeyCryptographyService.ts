export default class AuthenticatedSecretKeyCryptographyService {
  public static readonly KEY_LENGTH_IN_BYTES = 32;
  public static readonly SALT_LENGTH_IN_BYTES = 16;
  public static readonly TAG_LENGTH_IN_BYTES = 16;

  private readonly NONCE_LENGTH = 12;
  private readonly tagLengthInBytes: number;

  private readonly ALGORITHM = "AES-GCM";

  private cryptoKey!: CryptoKey;
  private derivedKey!: ArrayBuffer;
  private readonly masterKey: string | Uint8Array;

  private readonly salt: Uint8Array;
  private nonceBase!: ArrayBuffer;
  public seq: number;

  private readonly requirePassword: boolean;

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
        );
        this.masterKey = arrayKey.slice(
          AuthenticatedSecretKeyCryptographyService.SALT_LENGTH_IN_BYTES
        );
      } else {
        this.salt = arrayKey;
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

  async start() {
    this.cryptoKey = await this.getCryptoKeyFromRawKey(
      this.masterKey as string
    );
    this.nonceBase = await this.generateNonceBase();
  }

  async generateNonceBase() {
    const encoder = new TextEncoder();
    const inputKey = await crypto.subtle.importKey(
      "raw",
      this.derivedKey,
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

  generateNonce(seq: number) {
    if (seq > 0xffffffff) {
      throw new Error("record sequence number exceeds limit");
    }
    const nonce = new DataView(this.nonceBase.slice(0));
    const m = nonce.getUint32(nonce.byteLength - 4);
    const xor = (m ^ seq) >>> 0; //forces unsigned int xor
    nonce.setUint32(nonce.byteLength - 4, xor);
    return new Uint8Array(nonce.buffer);
  }

  public async getCryptoKeyFromRawKey(password: string) {
    this.derivedKey = this.requirePassword
      ? await this.derivePbkdfKeyMaterial(password)
      : await this.deriveHkdfKeyMaterial();
    return await crypto.subtle.importKey(
      "raw",
      this.derivedKey,
      {
        name: this.ALGORITHM,
      },
      true,
      ["encrypt", "decrypt"]
    );
  }

  public async encrypt(data: Uint8Array, seq: number): Promise<ArrayBuffer> {
    const nonce = this.generateNonce(seq);
    return await crypto.subtle.encrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      this.cryptoKey,
      data.buffer as ArrayBuffer
    );
  }

  public async decrypt(data: Uint8Array, seq: number): Promise<ArrayBuffer> {
    const nonce = this.generateNonce(seq);
    return await crypto.subtle.decrypt(
      {
        name: this.ALGORITHM,
        iv: nonce,
        tagLength: this.tagLengthInBytes * 8,
      },
      this.cryptoKey,
      data
    );
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
    const keyBuffer = await crypto.subtle.deriveBits(
      { name: "PBKDF2", salt: this.salt, iterations: 1e6, hash: "SHA-256" },
      keyMaterial,
      256
    );
    return keyBuffer;
  }

  private async deriveHkdfKeyMaterial() {
    const keyMaterial = await crypto.subtle.importKey(
      "raw",
      this.masterKey as Uint8Array,
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
      256 // 256 bits key length
    );
  }

  getSecret() {
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
}
