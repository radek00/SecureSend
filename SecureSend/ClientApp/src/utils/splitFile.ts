export default async function splitFile(
  file: File,
  chunkSize: number,
  callback: (
    chunk: Uint8Array,
    sequenceNumber: number,
    totalChunks: number
  ) => void,
  transformer?: (chunk: Uint8Array, chunkIndex: number) => any
) {
  const fileSize = file.size;
  const totalChunks = Math.ceil(fileSize / chunkSize);
  let offset = 0;

  while (offset < fileSize) {
    const buffer = await file.slice(offset, offset + chunkSize).arrayBuffer();
    await readEventHandler(buffer);
  }

  async function readEventHandler(buffer: ArrayBuffer) {
    const sequenceNumber = offset / chunkSize;
    offset += buffer.byteLength;
    let data = new Uint8Array(buffer);

    if (transformer) {
      data = await transformer(data, sequenceNumber);
      await callback(data, sequenceNumber, totalChunks);
    }
  }
}
