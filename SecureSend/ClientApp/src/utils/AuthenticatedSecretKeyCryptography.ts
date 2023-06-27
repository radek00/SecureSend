import { generateHash } from "./pbkdfHash";

export default class AuthenticatedSecretKeyCryptography {
    public static readonly KEY_LENGTH_IN_BYTES = 16;
    public static readonly IV_LENGTH_IN_BYTES = 16;
    public static readonly TAG_LENGTH_IN_BYTES = 16;
    private readonly NONCE_LENGTH = 12;
  
    private readonly ALGORITHM = 'AES-GCM';
    private secretKey!: CryptoKey;
    private readonly tagLengthInBytes: number;

    private salt: Uint8Array;
    private nonceBase!: ArrayBuffer;
    private password: string;
    public seq: number;
    public hash?: string;
  
    constructor(password: string,salt: Uint8Array, tagLengthInBytes = AuthenticatedSecretKeyCryptography.TAG_LENGTH_IN_BYTES) {
      this.tagLengthInBytes = tagLengthInBytes;
      this.salt = salt;
      this.password = password;
      this.seq = 0;
    }

    async start() {
      this.nonceBase = await this.generateNonceBase();
      this.secretKey = await this.getCryptoKeyFromRawKey(this.password)
    }

    async generateNonceBase() {
      const encoder  = new TextEncoder();
      const inputKey = await crypto.subtle.importKey(
        'raw',
        await this.deriveKeyMaterial(this.password, this.salt),
        'HKDF',
        false,
        ['deriveKey']
      );
  
      const base = await crypto.subtle.exportKey(
        'raw',
        await crypto.subtle.deriveKey(
          {
            name: 'HKDF',
            salt: this.salt,
            info: encoder.encode('Content-Encoding: nonce\0'),
            hash: 'SHA-256'
          },
          inputKey,
          {
            name: 'AES-GCM',
            length: 128
          },
          true,
          ['encrypt', 'decrypt']
        )
      );
  
      return base.slice(0, this.NONCE_LENGTH);
    }

    generateNonce(seq: number) {
      if (seq > 0xffffffff) {
        throw new Error('record sequence number exceeds limit');
      }
      const nonce = new DataView(this.nonceBase.slice(0));
      const m = nonce.getUint32(nonce.byteLength - 4);
      const xor = (m ^ seq) >>> 0; //forces unsigned int xor
      nonce.setUint32(nonce.byteLength - 4, xor);
      return new Uint8Array(nonce.buffer);
    }
  
    public async getCryptoKeyFromRawKey(password: string) {
    
      const key =  await crypto.subtle.importKey(
        'raw',
        await this.deriveKeyMaterial(password, this.salt),
        {
          name: this.ALGORITHM,
        },
        true,
        ['encrypt', 'decrypt'],
      );

      return key;
    }
  
    public async encrypt( data: Uint8Array, seq: number): Promise<ArrayBuffer> {
      const nonce = this.generateNonce(seq);
      const encrypted = await crypto.subtle.encrypt({
          name: this.ALGORITHM,
          iv: nonce,
          tagLength: this.tagLengthInBytes * 8,
        },
        this.secretKey,
        data.buffer,
      );
      return encrypted;
    }
  
    public async decrypt(data: Uint8Array, seq: number): Promise<ArrayBuffer> {
        const nonce = this.generateNonce(seq);
        const decrypted = await crypto.subtle.decrypt({
          name: this.ALGORITHM,
          iv: nonce,
          tagLength: this.tagLengthInBytes * 8,
        },
        this.secretKey,
        data,
      );
      return decrypted;
      
    }

    public async deriveKeyMaterial(password: string, salt: Uint8Array): Promise<ArrayBuffer> {
      const encoder = new TextEncoder();
      const keyMaterial = await crypto.subtle.importKey("raw", encoder.encode(password), "PBKDF2", false, ["deriveBits"]);
      const keyBuffer = await crypto.subtle.deriveBits({name: "PBKDF2", salt, iterations: 1e6, hash: "SHA-256"}, keyMaterial, 256);
      this.hash = generateHash(keyBuffer, this.salt);
      return keyBuffer;
    }
  }


  