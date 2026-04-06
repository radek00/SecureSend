/// <reference lib="WebWorker" />
import { IWorkerInit } from "./src/models/WorkerInit";

declare const self: ServiceWorkerGlobalScope;

import endpoints from "./src/config/endpoints";
import decryptStream from "./src/utils/streams/decryptionStream";

const map = new Map<string, IWorkerInit>();

self.addEventListener("install", () => {
  self.skipWaiting();
});

self.addEventListener("activate", (event) => {
  event.waitUntil(self.clients.claim());
});

const decrypt = async (id: string, url: string) => {
  const fileData = map.get(id);

  if (!fileData) return new Response(null, { status: 400 });

  try {
    const urlParams = new URL(url).searchParams;
    const fileName = urlParams.get("fileName");

    if (!fileName) {
      throw new Error("fileName parameter missing");
    }

    const metadata = fileData.metadata?.get(fileName);
    if (!metadata) {
      throw new Error("Metadata not found for file");
    }

    const fileResponse = await fetch(url);
    if (!fileResponse.ok) throw new Error(fileResponse.statusText);
    const body = fileResponse.body!;

    const decryptedResponse = decryptStream(
      body,
      fileData.b64key,
      +fileResponse.headers.get("Content-Length")!,
      fileName,
      fileData.password
    );

    const headers = {
      "Content-Disposition": `attachment; filename="${metadata.fileName}"`,
      "Content-Type": metadata.contentType ?? "application/octet-stream",
      "Content-Length": metadata.fileSize,
    };
    return new Response(decryptedResponse, { headers });
  } catch (error) {
    return new Response(null, { status: 500 });
  }
};

self.addEventListener("fetch", (event) => {
  if (event.request.method !== "GET") return;
  if (new URL(event.request.url).pathname === endpoints.download) {
    event.respondWith(
      decrypt(
        new URL(event.request.url).searchParams.get("id")!,
        event.request.url
      )
    );
  }
});

self.addEventListener("message", (event) => {
  if (event.data.request === "init") {
    map.set(event.data.id, { ...event.data });
  }
});
