import AuthenticatedSecretKeyCryptographyService from "../AuthenticatedSecretKeyCryptographyService";

export default class StreamDecryptor {
  private readonly keychain: AuthenticatedSecretKeyCryptographyService;

  constructor(b64Key: string, password?: string) {
    this.keychain = new AuthenticatedSecretKeyCryptographyService(
      password,
      b64Key
    );
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
