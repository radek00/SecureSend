import { test, expect, type Page } from "@playwright/test";
import { readFile } from "fs/promises";

test.describe("File upload and download flow", () => {
  const testFileName = "test-file.txt";
  const testFileContent = "This is a test file for SecureSend e2e testing";

  async function createTestFile(): Promise<{
    name: string;
    mimeType: string;
    buffer: Buffer;
  }> {
    return {
      name: testFileName,
      mimeType: "text/plain",
      buffer: Buffer.from(testFileContent),
    };
  }

  async function uploadFile(
    page: Page,
    withPassword: boolean = false,
    password: string = "test123"
  ) {
    await page.goto("/upload");

    if (withPassword) {
      await page.locator('input[name="isPasswordRequired"]').check();
      await page.locator('input[data-test="password"]').first().fill(password);
    }

    const mobileNextButton = page.locator('[data-test="next-upload-button"]');
    if (await mobileNextButton.isVisible()) {
      await mobileNextButton.click();
      await mobileNextButton.click();
    }

    const fileInput = page.locator('[data-test="file-input"]');
    await fileInput.setInputFiles(await createTestFile());

    const uploadButton = page
      .locator('[data-test="upload-button"]')
      .or(mobileNextButton);
    await uploadButton.click();

    await expect(
      page.locator('input[data-test="download-link-input"]')
    ).toBeVisible({ timeout: 15000 });
  }

  async function getDownloadLink(page: Page): Promise<string> {
    const linkInput = page.locator('input[data-test="download-link-input"]');
    await expect(linkInput).toBeVisible();
    const link = await linkInput.inputValue();
    expect(link).toContain("/download/");
    expect(link).toContain("#");
    return link;
  }

  test("upload file without password and download successfully", async ({
    page,
    context,
  }) => {
    await uploadFile(page, false);

    const downloadLink = await getDownloadLink(page);

    await page.locator('[data-test="close-modal-button"]').click();

    const downloadPage = await context.newPage();
    await downloadPage.goto(downloadLink);

    await expect(
      downloadPage.locator("h1", { hasText: "Download files" })
    ).toBeVisible({ timeout: 10000 });

    await expect(downloadPage.getByText(testFileName)).toBeVisible();

    const downloadPromise = downloadPage.waitForEvent("download");
    await downloadPage.locator('a[href*="download"]').first().click();
    const download = await downloadPromise;

    expect(download.suggestedFilename()).toBe(testFileName);

    const downloadPath = await download.path();

    expect(downloadPath).toBeTruthy();

    const downloadedData = await readFile(downloadPath!, "utf-8");
    expect(downloadedData).toBe(testFileContent);
  });

  test("upload file with password and download successfully", async ({
    page,
    context,
  }) => {
    const testPassword = "SecurePassword123!";

    await uploadFile(page, true, testPassword);

    const downloadLink = await getDownloadLink(page);

    await page.locator('[data-test="close-modal-button"]').click();

    const downloadPage = await context.newPage();
    await downloadPage.goto(downloadLink);

    await expect(
      downloadPage.locator("h2", { hasText: "Unlock your files" })
    ).toBeVisible();

    await downloadPage
      .locator('input[data-test="password"]')
      .fill(testPassword);
    await downloadPage.locator('button[type="submit"]').click();

    await expect(
      downloadPage.locator("h1", { hasText: "Download files" })
    ).toBeVisible({ timeout: 10000 });

    await expect(downloadPage.getByText(testFileName)).toBeVisible();

    const downloadPromise = downloadPage.waitForEvent("download");
    await downloadPage.locator('a[href*="download"]').first().click();
    const download = await downloadPromise;

    expect(download.suggestedFilename()).toBe(testFileName);

    const downloadPath = await download.path();
    expect(downloadPath).toBeTruthy();

    const downloadedData = await readFile(downloadPath!, "utf-8");
    expect(downloadedData).toBe(testFileContent);
  });

  test("upload fails with invalid password on download", async ({
    page,
    context,
  }) => {
    const testPassword = "SecurePassword123!";
    const wrongPassword = "WrongPassword";

    await uploadFile(page, true, testPassword);

    const downloadLink = await getDownloadLink(page);

    await page.locator('[data-test="close-modal-button"]').click();

    const downloadPage = await context.newPage();
    await downloadPage.goto(downloadLink);

    await expect(
      downloadPage.locator("h2", { hasText: "Unlock your files" })
    ).toBeVisible();

    await downloadPage
      .locator('input[data-test="password"]')
      .fill(wrongPassword);
    await downloadPage.locator('button[type="submit"]').click();

    await expect(
      downloadPage.locator('[data-test="error-message"]', {
        hasText: "Invalid password",
      })
    ).toBeVisible({ timeout: 5000 });
  });

  test("copy link to clipboard works", async ({ page, context }) => {
    await uploadFile(page, false);

    try {
      await context.grantPermissions(["clipboard-read", "clipboard-write"]);
    } catch (error) {
      console.log("Permission granting not supported on this browser");
    }

    await page.locator('[data-test="copy-link-button"]').click();

    const clipboardText = await page.evaluate(() =>
      navigator.clipboard.readText()
    );
    expect(clipboardText).toContain("/download/");
    expect(clipboardText).toContain("#");
  });

  test("upload multiple files", async ({ page, context }) => {
    await page.goto("/upload");

    const mobileNextButton = page.locator('[data-test="next-upload-button"]');
    if (await mobileNextButton.isVisible()) {
      await mobileNextButton.click();
      await mobileNextButton.click();
    }

    const fileInput = page.locator('[data-test="file-input"]');

    const files = [
      {
        name: "file1.txt",
        mimeType: "text/plain",
        buffer: Buffer.from("Content 1"),
      },
      {
        name: "file2.txt",
        mimeType: "text/plain",
        buffer: Buffer.from("Content 2"),
      },
      {
        name: "file3.txt",
        mimeType: "text/plain",
        buffer: Buffer.from("Content 3"),
      },
    ];

    await fileInput.setInputFiles(files);

    const uploadButton = page
      .locator('[data-test="upload-button"]')
      .or(mobileNextButton);
    await uploadButton.click();

    await expect(
      page.locator('input[data-test="download-link-input"]')
    ).toBeVisible({ timeout: 15000 });

    const downloadLink = await getDownloadLink(page);

    await page.locator('[data-test="close-modal-button"]').click();

    const downloadPage = await context.newPage();
    await downloadPage.goto(downloadLink);

    await expect(
      downloadPage.locator("h1", { hasText: "Download files" })
    ).toBeVisible({ timeout: 10000 });

    await expect(downloadPage.getByText("file1.txt")).toBeVisible();
    await expect(downloadPage.getByText("file2.txt")).toBeVisible();
    await expect(downloadPage.getByText("file3.txt")).toBeVisible();
  });
});
