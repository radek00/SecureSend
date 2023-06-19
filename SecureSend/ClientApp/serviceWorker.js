import endpoints from "@/config/endpoints";
const map = new Map();

self.addEventListener('install', () => {
  console.log('sw installing')
    self.skipWaiting();
});

self.addEventListener('activate', event => {
  console.log('sw active')
    event.waitUntil(self.clients.claim());
});


self.addEventListener('fetch', (event) => {
  if (event.request.method !== 'GET') return;
  if (new URL(event.request.url).pathname === endpoints.download) {
    console.log('fetching')
  }
})

self.addEventListener('message', (event) => {
  if (event.data.request === 'init') {
    console.log('download init');
    map.set(event.data.id, {...event.data})
    console.log(map)
  }
})