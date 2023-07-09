import { fileURLToPath, URL } from "node:url";

import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import mkcert from "vite-plugin-mkcert";

const baseConfig = {
  plugins: [vue(), mkcert()],
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
        rewrite: (path: string) => path.replace(/^\/api/, "/api"),
      },
    },
  },
};

export default defineConfig(({ mode }): any => {
  if (mode === "app") {
    return {
      ...baseConfig,
      build: {
        emptyOutDir: true,
        rollupOptions: {
          input: {
            app: "./index.html",
          },
          output: {
            entryFileNames: () => "assets/js/[name]-[hash].js",
            inlineDynamicImports: false,
          },
        },
      },
    };
  }

  if (mode === "worker") {
    return {
      ...baseConfig,
      build: {
        emptyOutDir: false,
        rollupOptions: {
          input: {
            serviceWorker: "./serviceWorker.js",
          },
          output: {
            entryFileNames: () => "[name].js",
            inlineDynamicImports: true,
          },
        },
      },
    };
  }
  return baseConfig;
});
