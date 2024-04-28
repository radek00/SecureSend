import { VueWrapper, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent, waitForExpect } from "@/tests/utils";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import SizeLimit from "@/components/FileUploadForm/SizeLimit.vue";

describe("Form validation when size limits are set", () => {
  let wrapper: VueWrapper<any>;
  beforeEach(async () => {
    vi.setSystemTime(new Date(2022, 1, 20));
    UploadLimitsService.getUploadLimits = vi
      .fn()
      .mockReturnValue({ singleUploadLimitInGB: 1, maxExpirationInDays: 3 });
    wrapper = mountComponent(FileUploadView).wrapper;
    await flushPromises();
    await wrapper.vm.$nextTick();
  });

  test("Upload size limits", async () => {
    const submitButton = wrapper.find('button[type="submit"]');
    await submitButton.trigger("submit");

    const dateInput = wrapper.find('input[type="date"]');
    await dateInput.setValue("2022-02-21");

    await submitButton.trigger("submit");

    const fileInputComponent = wrapper.findComponent(FileInput);
    const file = new File(["file content"], "test.txt", { type: "text/plain" });
    Object.defineProperty(file, "size", { value: 5 });
    fileInputComponent.vm.$emit("onFilesChange", [file]);
    await wrapper.vm.$nextTick();
    await waitForExpect(() => {
      expect(
        fileInputComponent
          .findComponent(SizeLimit)
          .find(".text-red-500")
          .exists()
      ).toBe(false);
      expect(submitButton.attributes("disabled")).toBeUndefined();
    });

    const file2 = new File(["file content2"], "test2.txt", {
      type: "text/plain",
    });
    Object.defineProperty(file2, "size", { value: 1024 * 1024 * 1024 * 1024 });
    fileInputComponent.vm.$emit("onFilesChange", [file2]);
    await wrapper.vm.$nextTick();
    await waitForExpect(() => {
      expect(submitButton.attributes("disabled")).toBeDefined();
      expect(
        fileInputComponent
          .findComponent(SizeLimit)
          .find(".text-red-500")
          .exists()
      ).toBe(true);
    });
  });

  test("Max date validation when max expiration limit is set", async () => {
    expect(wrapper.text()).toContain("Expire after");
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");

    const dateInput = wrapper.find('input[type="date"]');
    await dateInput.setValue("2024-02-24");
    await flushPromises();
    await waitForExpect(() => {
      expect(
        wrapper
          .find('div[data-test="expirationDate"] p[data-test="error-message"]')
          .text()
      ).toEqual("Max allowed expiration date is: 2022-02-23");
    }, 500);
  });
});
