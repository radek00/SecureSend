// This source code is taken from Firefox Send (https://github.com/mozilla/send) and slightly modified.

export default class StreamSlicer {
  protected chunkSize: number;
  protected partialChunk: Uint8Array;
  protected offset: number;

  constructor(chunkSize: number) {
    this.chunkSize = chunkSize;
    this.partialChunk = new Uint8Array(this.chunkSize);
    this.offset = 0;
  }

  public send(buf: Uint8Array, controller: TransformStreamDefaultController) {
    controller.enqueue(buf);
    this.partialChunk = new Uint8Array(this.chunkSize);
    this.offset = 0;
  }

  public transform(
    chunk: Uint8Array,
    controller: TransformStreamDefaultController
  ) {
    let i = 0;

    if (this.offset > 0) {
      const len = Math.min(chunk.byteLength, this.chunkSize - this.offset);
      this.partialChunk.set(chunk.slice(0, len), this.offset);
      this.offset += len;
      i += len;

      if (this.offset === this.chunkSize) {
        this.send(this.partialChunk, controller);
      }
    }

    while (i < chunk.byteLength) {
      const remainingBytes = chunk.byteLength - i;
      if (remainingBytes >= this.chunkSize) {
        const record = chunk.slice(i, i + this.chunkSize);
        i += this.chunkSize;
        this.send(record, controller);
      } else {
        const end = chunk.slice(i, i + remainingBytes);
        i += end.byteLength;
        this.partialChunk.set(end);
        this.offset = end.byteLength;
      }
    }
  }

  public flush(controller: TransformStreamDefaultController) {
    if (this.offset > 0) {
      controller.enqueue(this.partialChunk.slice(0, this.offset));
    }
  }
}
