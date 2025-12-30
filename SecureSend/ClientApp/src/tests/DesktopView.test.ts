import { VueWrapper, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent } from "@/tests/utils";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import { ref } from "vue";
import FileInput from "@/components/FileUploadForm/FileInput.vue";

describe("Desktop view tests", () => {
  let wrapper: VueWrapper<any>;
  vi.mock("@/utils/composables/useScreenSize", async (importOriginal) => {
    const actual =
      await importOriginal<
        typeof import("@/utils/composables/useScreenSize")
      >();
    return {
      ...actual,
      useScreenSize: () => ({
        screenType: ref(actual.ScreenType.LG),
      }),
    };
  });
  beforeEach(async () => {
    vi.setSystemTime(new Date(2022, 1, 20));
    UploadLimitsService.getUploadLimits = vi.fn().mockReturnValue({});
    wrapper = mountComponent(FileUploadView).wrapper;
    await flushPromises();
    await wrapper.vm.$nextTick();
  });

  test("Desktop view has all controls present", async () => {
    const passwordInput = wrapper.find('input[data-test="password"]');
    expect(passwordInput.exists()).toBe(true);

    const requiredCheckbox = wrapper.find('input[type="checkbox"]');
    expect(requiredCheckbox.exists()).toBe(true);

    const expiryDateInput = wrapper.find('input[data-test="expirationDate"]');
    expect(expiryDateInput.exists()).toBe(true);

    const resetButton = wrapper.find('button[data-test="reset-button"]');
    expect(resetButton.exists()).toBe(true);
    expect(resetButton.attributes("disabled")).toBeDefined();

    const fileUploadComponent = wrapper.findComponent(FileInput);
    expect(fileUploadComponent.exists()).toBe(true);

    const uploadButton = wrapper.find('button[type="submit"]');
    expect(uploadButton.exists()).toBe(true);
    expect(uploadButton.attributes("disabled")).toBeDefined();
    expect(uploadButton.text()).toEqual("Upload");
  });
});
