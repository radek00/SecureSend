import { test, expect } from "@playwright/test";

test.describe.parallel("Service worker validation", () => {
  test("should register service worker", async ({ page }) => {
    await page.goto("/");
    const serviceWorker = await page.evaluate(() => {
      return navigator.serviceWorker.getRegistration();
    });
    expect(serviceWorker).not.toBeNull();
  });
});
