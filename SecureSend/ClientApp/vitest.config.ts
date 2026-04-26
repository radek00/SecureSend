import { defineConfig } from "vitest/config";
import { resolve } from "node:path";
import vue from "@vitejs/plugin-vue";

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: "happy-dom",
    setupFiles: ["./src/tests/setup.ts"],
    include: ["src/tests/**/*.test.ts"],
    clearMocks: true,
  },
  resolve: {
    alias: [{ find: "@", replacement: resolve(__dirname, "./src") }],
  },
});
