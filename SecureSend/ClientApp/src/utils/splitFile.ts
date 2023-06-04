// This source code is taken from Firefox Send (https://github.com/mozilla/send) and slightly modified.

export default async function splitFile(
    file: File,
    chunkSize: number,
    callback: (chunk: Uint8Array, sequenceNumber: number, totalChunks: number) => void,
    transformer?: (chunk: Uint8Array, chunkIndex: number) => any,
  ) {
    const fileSize = file.size;
    const totalChunks = Math.ceil(fileSize / chunkSize);
    console.log(totalChunks)
    let offset = 0;
  
    const readChunk = () => {
      return new Promise((resolve, reject) => {
        const r = new FileReader();
        const blob = file.slice(offset, chunkSize + offset);
        r.onload = async(evt) => {
          const execute = async() => {
            try {
              await readEventHandler(evt);
            } catch (error) {
              reject(error)
            }
          }
          resolve(await execute());
        };
        r.readAsArrayBuffer(blob);
      })

    };
  
    const readEventHandler = async (evt: any) => {
      if (evt.target.error == null) {
        const sequenceNumber = (offset / chunkSize);
        offset += evt.target.result.byteLength;
        let data = new Uint8Array(evt.target.result);
  
        if (transformer) {

            data = await transformer(data, sequenceNumber);
            await callback(data, sequenceNumber, totalChunks);
        }
        
        
      } else {
        // Read error.
        return;
      }
  
      if (offset >= fileSize) {
        // Done reading file.
        return;
      }
  
      // Off to the next chunk.
      await readChunk();
    };
  
    // Let's start reading the first block.
    await readChunk();
  }
  