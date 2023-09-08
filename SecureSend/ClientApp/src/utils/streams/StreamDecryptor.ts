import AuthenticatedSecretKeyCryptography from "../AuthenticatedSecretKeyCryptography";
import type {encryptionKey} from "@/models/utilityTypes/encryptionKey";

export default class StreamDecryptor {
  private readonly keychain: AuthenticatedSecretKeyCryptography;

  constructor(masterKey: encryptionKey, salt: Uint8Array) {
    this.keychain = new AuthenticatedSecretKeyCryptography(salt, masterKey);
  }

  public async transform(
    chunk: Uint8Array,
    controller: TransformStreamDefaultController
  ) {
    try {
      if (this.keychain.seq === 0) {
        await this.keychain.start();
      }
      const decryptedChunk = await this.keychain.decrypt(
        chunk,
        this.keychain.seq
      );
      controller.enqueue(new Uint8Array(decryptedChunk));
      this.keychain.seq++;
    } catch (error) {
      controller.error(error);
    }
  }
}
