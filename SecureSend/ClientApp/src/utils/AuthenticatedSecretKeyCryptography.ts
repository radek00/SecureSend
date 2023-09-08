import {generateHash} from "./pbkdfHash";
import type {encryptionKey} from "@/models/utilityTypes/encryptionKey";


export default class AuthenticatedSecretKeyCryptography {
  public static readonly KEY_LENGTH_IN_BYTES = 16;
  public static readonly IV_LENGTH_IN_BYTES = 16;
  public static readonly TAG_LENGTH_IN_BYTES = 16;
  private readonly NONCE_LENGTH = 12;

  private readonly ALGORITHM = "AES-GCM";
  private secretKey!: CryptoKey;
  private keyData!: ArrayBuffer;
  private readonly tagLengthInBytes: number;

  private readonly salt: Uint8Array;
  private nonceBase!: ArrayBuffer;
  public seq: number;
  private hash?: string;
  private readonly masterKey: encryptionKey;

  private readonly requirePassword: boolean;

  constructor(
    salt: Uint8Array,
    password?: encryptionKey,
    tagLengthInBytes = AuthenticatedSecretKeyCryptography.TAG_LENGTH_IN_BYTES
  ) {
    this.tagLengthInBytes = tagLengthInBytes;
    this.salt = salt;
    this.masterKey = password ? password : crypto.getRandomValues(new Uint8Array(32));
    this.seq = 0;
    this.requirePassword = typeof this.masterKey === 'string';
  }

  async start() {
    this.secretKey = await this.getCryptoKeyFromRawKey(this.masterKey);
    this.nonceBase = await this.generateNonceBase();
  }

  async generateNonceBase() {
    const encoder = new TextEncoder();
    const inputKey = await crypto.subtle.importKey(
      "raw",
      this.keyData,
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

  public async getCryptoKeyFromRawKey(masterKey: encryptionKey) {
    const keyData = this.requirePassword ? await this.derivePbkdfKeyMaterial(masterKey as string, this.salt) : await this.deriveHkdfKeyMaterial();
    return await crypto.subtle.importKey(
        "raw",
        keyData
        ,
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
        this.secretKey,
        data.buffer
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
        this.secretKey,
        data
    );
  }

  private async derivePbkdfKeyMaterial(
    password: string,
    salt: Uint8Array
  ): Promise<ArrayBuffer> {
    const encoder = new TextEncoder();
    const keyMaterial = await crypto.subtle.importKey(
      "raw",
      encoder.encode(password),
      "PBKDF2",
      false,
      ["deriveBits"]
    );
    const keyBuffer = await crypto.subtle.deriveBits(
      { name: "PBKDF2", salt, iterations: 1e6, hash: "SHA-256" },
      keyMaterial,
      256
    );
    this.keyData = keyBuffer;
    return keyBuffer;
  }

  private async deriveHkdfKeyMaterial() {
    const keyMaterial = await crypto.subtle.importKey('raw', this.masterKey as Uint8Array, 'HKDF', false, ['deriveBits']);
    const info = new TextEncoder().encode('info');
    this.keyData =  await crypto.subtle.deriveBits(
        {
          name: 'HKDF',
          salt: this.salt,
          info,
          hash: "SHA-256",
        },
        keyMaterial,
        256 // 256 bits key length
    );
    return this.keyData;
  }

  getMasterKey() {
    return this.masterKey as Uint8Array;
  }

  getHash() {
      if (this.hash) return this.hash;
      this.hash = generateHash(this.keyData, this.salt);
      return this.hash;
  }
}
