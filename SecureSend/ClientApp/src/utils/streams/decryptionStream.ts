import StreamSlicer from "./StreamSlicer";
import StreamDecryptor from "./StreamDecryptor";
import type { encryptionKey } from "@/models/utilityTypes/encryptionKey";

export default function decryptStream(
  input: ReadableStream,
  b64Key: string,
  password?: string
) {
  const inputStream = input.pipeThrough(
    new TransformStream(new StreamSlicer(5 * 1024 * 1024 + 16))
  );
  return inputStream.pipeThrough(
    new TransformStream(new StreamDecryptor(b64Key, password))
  );
}
