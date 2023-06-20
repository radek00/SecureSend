import endpoints from "@/config/endpoints.ts";
import decryptStream from "@/utils/streams/decryptionStream.ts"

const map = new Map();

self.addEventListener('install', () => {
  console.log('sw installing')
    self.skipWaiting();
});

self.addEventListener('activate', event => {
  console.log('sw active')
    event.waitUntil(self.clients.claim());
});

const decrypt = async(id, url) => {
  console.log(id, url)
  const fileData = map.get(id);

  if (!fileData) return new Response(null, {status: 400});

  try {
    const fileResponse = await fetch(url);
    const body = fileResponse.body;
    const decryptedResponse = decryptStream(body, fileData.salt, "password");
    const headers = {
      'Content-Disposition': fileResponse.headers.get('Content-Disposition'),
      'Content-Type': fileResponse.headers.get('Content-Type'),
      'Content-Length': fileResponse.headers.get('Content-Length')
    };
    return new Response(decryptedResponse, {headers})
  } catch (error) {
    return new Response(null, {status: error.message})
  }
}


self.addEventListener('fetch', (event) => {
  if (event.request.method !== 'GET') return;
  if (new URL(event.request.url).pathname === endpoints.download) {
    console.log('fetching', event.request)
    event.respondWith(decrypt(new URL(event.request.url).searchParams.get('id'), event.request.url))
  }
})

self.addEventListener('message', (event) => {
  if (event.data.request === 'init') {
    console.log('download init');
    map.set(event.data.id, {...event.data})
    console.log(map)
  }
})