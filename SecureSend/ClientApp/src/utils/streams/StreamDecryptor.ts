import AuthenticatedSecretKeyCryptography from "../AuthenticatedSecretKeyCryptography";

export default class StreamDecryptor {
    private readonly keychain: AuthenticatedSecretKeyCryptography;

    constructor(password: string, salt: Uint8Array) {
        this.keychain = new AuthenticatedSecretKeyCryptography(password, salt);
    }

    public async transform(chunk: any, controller: any) {
        if (this.keychain.seq === 0) {
          await this.keychain.start();
        } 
        console.log('transforming')
          const decryptedChunk = await this.keychain.decrypt(chunk, this.keychain.seq);
            controller.enqueue(
              new Uint8Array(decryptedChunk)
            )
          
  
        
        
        this.keychain.seq++;
      }
  
      async flush(controller: any) {
        console.log('ece stream ends')
        // if (this.prevChunk) {
        //   await this.transformPrevChunk(true, controller);
        // }
      }
}