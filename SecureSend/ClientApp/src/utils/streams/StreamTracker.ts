export default class StreamTracker {
  private readonly totalChunks: number;
  private currentChunk = 0;
  private readonly fileName: string;
  private readonly broadcast = new BroadcastChannel("progress-channel");
  constructor(totalSize: number, fileName: string) {
    this.fileName = fileName;
    this.totalChunks = Math.ceil(totalSize / (5 * 1024 * 1024 + 16));
  }

  public async transform(
    chunk: Uint8Array,
    controller: TransformStreamDefaultController
  ) {
    try {
      this.broadcast.postMessage({
        request: "progress",
        value: `${Math.ceil((this.currentChunk / this.totalChunks) * 100)}%`,
        fileName: this.fileName,
      });
      this.currentChunk++;
      controller.enqueue(chunk);
    } catch (error) {
      controller.error(error);
    }
  }

  public flush() {
    this.broadcast.postMessage({
      request: "progress",
      value: `${Math.ceil((this.currentChunk / this.totalChunks) * 100)}%`,
      fileName: this.fileName,
    });
    this.broadcast.close();
  }
}
