import { fileURLToPath, URL } from "node:url";

import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import mkcert from "vite-plugin-mkcert";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), mkcert()],
  build: {
    rollupOptions: {
      input: {
        app: './index.html',
        serviceWorker: './serviceWorker.js'
      },
      output: {
        entryFileNames: assetInfo => {
          return assetInfo.name === 'serviceWorker'
             ? '[name].js'                  // put service worker in root
             : 'assets/js/[name]-[hash].js' // others in `assets/js/`
        }
      },
    }
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    port: 3000,
    strictPort: true,
    https: true,
    proxy: {
      "/api": {
        target: "https://localhost:7109",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, "/api"),
      },
    },
  },
});
