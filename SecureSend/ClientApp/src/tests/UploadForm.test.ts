import { VueWrapper, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent, waitForExpect } from "@/tests/utils";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import { HTMLInputElement } from "happy-dom";

describe("Upload form", () => {
  let wrapper: VueWrapper<any>;
  beforeEach(async () => {
    vi.setSystemTime(new Date(2022, 1, 20));
    UploadLimitsService.getUploadLimits = vi.fn().mockReturnValue({});
    wrapper = mountComponent(FileUploadView).wrapper;
    await flushPromises();
    await wrapper.vm.$nextTick();
  });

  test("Multistep form flow", async () => {
    const submitButton = wrapper.find('button[type="submit"]');
    const passwordRequired = wrapper.find('input[type="password"]');

    const submit = vi.spyOn(wrapper.vm, "onSubmit");

    expect(passwordRequired.attributes("disabled")).toBeDefined();
    expect(submitButton.attributes("disabled")).toBeUndefined();
    expect(submitButton.text()).toEqual("Next");

    await submitButton.trigger("submit");
    expect(submit).toHaveBeenCalledTimes(1);

    await submitButton.trigger("submit");
    expect(submit).toHaveBeenCalledTimes(2);

    await flushPromises();
    await waitForExpect(() => {
      expect(submitButton.text()).toEqual("Upload");
      expect(submitButton.attributes("disabled")).toBeDefined();
    });
  });

  test("Password validation", async () => {
    const passwordInput = wrapper.find('input[type="password"]');
    expect(passwordInput.attributes("disabled")).toBeDefined();

    const requiredCheckbox = wrapper.find('input[type="checkbox"]');
    await requiredCheckbox.setValue();

    await waitForExpect(() => {
      expect(passwordInput.attributes("disabled")).toBeUndefined();
    });

    await passwordInput.setValue("test");
    await passwordInput.setValue("");

    await waitForExpect(() => {
      expect(
        wrapper.find('div[data-test="password"]').find("span").text()
      ).toEqual("Password is required.");
    });
  });

  test("Date validation", async () => {
    expect(wrapper.text()).toContain("Optional expiration date");
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");

    const dateInput = wrapper.find('input[type="date"]');

    //check default value
    expect((dateInput.element as unknown as HTMLInputElement).value).toEqual("2022-02-21");

    await dateInput.setValue("2021-12-01");
    await waitForExpect(async () => {
      expect(
        wrapper
          .find('div[data-test="expirationDate"] p[data-test="error-message"]')
          .text()
      ).toEqual("Expiry date must be later than today.");
    });
  });
});
