import StreamSlicer from "./StreamSlicer";
import StreamDecryptor from "./StreamDecryptor";
import StreamTracker from "@/utils/streams/StreamTracker";

export default function decryptStream(
  input: ReadableStream,
  b64Key: string,
  fileSize: number,
  fileName: string,
  password?: string
) {
  const inputStream = input.pipeThrough(
    new TransformStream(new StreamSlicer(5 * 1024 * 1024 + 16))
  );
  const decryptedStream = inputStream.pipeThrough(
    new TransformStream(new StreamDecryptor(b64Key, password))
  );

  return decryptedStream.pipeThrough(
    new TransformStream(new StreamTracker(fileSize, fileName))
  );
}
