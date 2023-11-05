export default async function splitFile(
  file: File,
  chunkSize: number,
  callback: (
    chunk: ArrayBuffer,
    sequenceNumber: number,
    totalChunks: number
  ) => Promise<void>,
  transformer: (chunk: Uint8Array, chunkIndex: number) => Promise<ArrayBuffer>
) {
  const fileSize = file.size;
  const totalChunks = Math.ceil(fileSize / chunkSize);
  let offset = 0;

  if (fileSize === 0) {
    await callback(await file.arrayBuffer(), 0, 1);
    return;
  }

  while (offset < fileSize) {
    const buffer = await file.slice(offset, offset + chunkSize).arrayBuffer();
    await readEventHandler(buffer);
  }

  async function readEventHandler(buffer: ArrayBuffer) {
    const sequenceNumber = offset / chunkSize;
    offset += buffer.byteLength;
    const data = await transformer(new Uint8Array(buffer), sequenceNumber);
    await callback(data, sequenceNumber, totalChunks);
  }
}
