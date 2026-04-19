import { test, expect } from "@playwright/test";

test.describe.parallel("Service worker validation", () => {
  test("should register service worker", async ({ page }) => {
    await page.goto("/");
    const serviceWorker = await page.evaluate(() => {
      return navigator.serviceWorker.getRegistration();
    });
    expect(serviceWorker).not.toBeNull();
  });

  test("service worker receives ping every 10 seconds", async ({ page }) => {
    await page.goto("/");
    const pingReceived = await page.evaluate(() => {
      return new Promise((resolve) => {
        navigator.serviceWorker.addEventListener("message", (event) => {
          if (event.data === "pong") {
            resolve(true);
          }
        });
      });
    });
    expect(pingReceived).toBe(true);
  });
});
