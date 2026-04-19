import { defineConfig, devices } from "@playwright/test";
import playwrightConfig from "./playwright.config";

export default defineConfig({
    ...playwrightConfig,
     use: {
    baseURL: "https://localhost:3000/",
    trace: "on-first-retry",
  },
  projects: [
    {
      name: "Desktop Chrome",
      use: { ...devices["Desktop Chrome"] },
    },
    {
      name: "Mobile Chrome",
      use: { ...devices["Pixel 5"] },
    },
  ],
})