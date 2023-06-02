import AuthenticatedSecretKeyCryptography from "../AuthenticatedSecretKeyCryptography"
import StreamSlicer from "./StreamSlicer"

export default function decryptStream(input: ReadableStream, salt: Uint8Array) {
    console.log('streaming')
    const inputStream = input.pipeThrough(new TransformStream(new StreamSlicer((64 * 1024) + 16)))
    return inputStream.pipeThrough(new TransformStream(new AuthenticatedSecretKeyCryptography("password", salt)))
  }