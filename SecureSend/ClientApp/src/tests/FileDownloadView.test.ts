import { flushPromises, VueWrapper } from "@vue/test-utils";
import { mountComponent } from "@/tests/utils";
import type { Router } from "vue-router";
import { beforeEach, describe, expect, test, vi } from "vitest";
import App from "@/App.vue";
import { SecureSendService } from "@/services/SecureSendService";
import {
  InvalidPasswordError,
  UploadExpiredError,
} from "@/models/errors/ResponseErrors";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";

describe("FileDownloadView", () => {
  let wrapper: VueWrapper<any>;
  let router: Router;
  const link =
    "/download/280a2753-5b23-4fbd-83fe-b080379c3ad2#Jdow6mtFDKxPRY5hpFphXf9Voj8ffJvXtSDTMxUl38E/AK99GPot44E31Kp9nsjE";
  beforeEach(async () => {
    vi.stubGlobal("navigator", {
      serviceWorker: {
        controller: {
          postMessage: () => {},
        },
      },
    });
    const component = mountComponent(App);
    wrapper = component.wrapper;
    router = component.router;
    await flushPromises();
  });

  test("Redirects to error page with generic message when upload is not found or something went wrong", async () => {
    SecureSendService.verifySecureUpload = vi
      .fn()
      .mockRejectedValue(new Error("Upload does not exist"));
    await router.push(link);

    expect(wrapper.html()).toContain("Something's missing.");
  });
  test("Redirects to error page with expired message when upload is expired", async () => {
    SecureSendService.verifySecureUpload = vi
      .fn()
      .mockRejectedValue(new UploadExpiredError("Upload expired"));
    await router.push(link);

    expect(wrapper.html()).toContain("Upload expired.");
  });

  test("FileDownloadView component asks for password when upload is protected and lists files after unlock", async () => {
    SecureSendService.verifySecureUpload = vi.fn().mockReturnValue({
      secureUploadId: "280a2753-5b23-4fbd-83fe-b080379c3ad2",
      isProtected: true,
    } as UploadVerifyResponseDTO);
    await router.push(link);
    await flushPromises();
    expect(wrapper.html()).toContain("Unlock your files");

    const unlockButton = wrapper.find("button");
    SecureSendService.viewSecureUpload = vi.fn().mockReturnValue({
      secureUploadId: "280a2753-5b23-4fbd-83fe-b080379c3ad2",
      uploadDate: "2023-12-12",
      expiryDate: null,
      files: [
        {
          fileName: "test.txt",
          contentType: "application/text",
          fileSize: 100,
        },
      ],
    });
    await unlockButton.trigger("submit");
    await wrapper.vm.$nextTick();
    expect(wrapper.html()).toContain("test.txt");
    expect(wrapper.find("a").attributes("href")).toEqual(
      "/api/SecureSend/download?id=280a2753-5b23-4fbd-83fe-b080379c3ad2&fileName=test.txt"
    );
  });

  test("FileDownloadView password validation", async () => {
    SecureSendService.viewSecureUpload = vi
      .fn()
      .mockRejectedValue(new InvalidPasswordError("Invalid password"));
    await router.push(link);
    await flushPromises();

    const passwordInput = wrapper.find('input[type="password"]');
    const unlockButton = wrapper.find("button");
    await passwordInput.setValue("test");

    await unlockButton.trigger("click");
    expect(wrapper.html()).toContain("Invalid password");
  });
});
