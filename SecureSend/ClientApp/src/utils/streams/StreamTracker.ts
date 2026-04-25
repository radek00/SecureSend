export default class StreamTracker {
  private readonly totalBytes: number;
  private processedBytes = 0;
  private readonly fileName: string;
  private readonly broadcast = new BroadcastChannel("progress-channel");
  constructor(totalSize: number, fileName: string) {
    this.fileName = fileName;
    this.totalBytes = Math.max(1, totalSize);
  }

  public async transform(
    chunk: Uint8Array,
    controller: TransformStreamDefaultController
  ) {
    try {
      controller.enqueue(chunk);
      this.processedBytes += chunk.byteLength;
      this.broadcast.postMessage({
        request: "progress",
        value: `${Math.min(Math.floor((this.processedBytes / this.totalBytes) * 100), 99)}%`,
        fileName: this.fileName,
      });
    } catch (error) {
      this.broadcast.close();
      controller.error(error);
    }
  }

  public flush() {
    this.broadcast.postMessage({
      request: "progress",
      value: "100%",
      fileName: this.fileName,
    });
    this.broadcast.close();
  }
}
