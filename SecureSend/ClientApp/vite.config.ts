import { fileURLToPath, URL } from "node:url";

import { defineConfig, UserConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import mkcert from "vite-plugin-mkcert";
import packageJson from "./package.json";

const baseConfig: UserConfig = {
  plugins: [
    vue(),
    mkcert(),
    {
      name: "html-transform",
      transformIndexHtml(html) {
        return html.replace(/%APP_VERSION%/g, packageJson.version);
      },
    },
  ],
  envDir: "./environment",
  base: "/",
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

export default defineConfig((config: UserConfig): UserConfig => {
  if (config.mode === "production") {
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

  if (config.mode === "worker") {
    return {
      ...baseConfig,
      build: {
        emptyOutDir: false,
        rollupOptions: {
          input: {
            serviceWorker: "./serviceWorker.ts",
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
