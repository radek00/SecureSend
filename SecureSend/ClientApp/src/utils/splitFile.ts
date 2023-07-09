// This source code is taken from Firefox Send (https://github.com/mozilla/send) and slightly modified.

export default async function splitFile(
  file: File,
  chunkSize: number,
  callback: (chunk: Uint8Array, sequenceNumber: number, totalChunks: number) => void,
  transformer?: (chunk: Uint8Array, chunkIndex: number) => any,
) {
  return new Promise((resolve, reject) => {

    const fileSize = file.size;
    const totalChunks = Math.ceil(fileSize / chunkSize);
    console.log(totalChunks)
    let offset = 0;
  
    const readChunk = () => {
      const r = new FileReader();
      const blob = file.slice(offset, chunkSize + offset);
      r.onload = readEventHandler;
      r.readAsArrayBuffer(blob);
    };
  
    const readEventHandler = async (evt: any) => {
      if (evt.target.error == null) {
        const sequenceNumber = (offset / chunkSize);
        offset += evt.target.result.byteLength;
        let data = new Uint8Array(evt.target.result);
  
        if (transformer) {
          try {
            data = await transformer(data, sequenceNumber);
            await callback(data, sequenceNumber, totalChunks);
          } catch (error) {
            reject(error);
          }
        }
        
        
      } else {
        // Read error.
        return;
      }
  
      if (offset >= fileSize) {
        // Done reading file.
        resolve(undefined);
        return;
      }
  
      // Off to the next chunk.
      readChunk();
    };
  
    // Let's start reading the first block.
    readChunk();
  })
}
