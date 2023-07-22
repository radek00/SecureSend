import AuthenticatedSecretKeyCryptography from "../AuthenticatedSecretKeyCryptography";

export default class StreamDecryptor {
  private readonly keychain: AuthenticatedSecretKeyCryptography;

  constructor(password: string, salt: Uint8Array) {
    this.keychain = new AuthenticatedSecretKeyCryptography(password, salt);
  }

  public async transform(chunk: any, controller: any) {
    try {
      if (this.keychain.seq === 0) {
        await this.keychain.start();
      }
      console.log("transforming");
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
