import { VueWrapper, mount, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView.vue";
import { ref } from "vue";
import { waitForExpect } from "@/tests/utils";

describe("FileUploadView", () => {
  let wrapper: VueWrapper<any>;
  beforeEach(async () => {
    const div = document.createElement("div");
    div.id = "root";
    document.body.appendChild(div);
    wrapper = mount(FileUploadView, {
      global: { provide: { isLoading: ref(false) } },
    });
    await flushPromises();
  });

  test("Multistep form flow", async () => {
    const submitButton = wrapper.find('button[type="submit"]');
    console.log(submitButton.text());
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
      console.log(wrapper.vm.currentSchema);
      expect(submitButton.text()).toEqual("Upload");
      expect(submitButton.attributes("disabled")).toBeDefined();
    });
  });

  test("Password validation", async () => {
    const passwordInput = wrapper.find('input[type="password"]');
    expect(passwordInput.attributes("disabled")).toBeDefined();

    const requiredCheckbox = wrapper.find('input[name="isPasswordRequired"');
    expect(requiredCheckbox.attributes("value")).toEqual("false");

    await requiredCheckbox.setValue();

    await waitForExpect(() => {
      expect(passwordInput.attributes("disabled")).toBeUndefined();
    });

    await passwordInput.setValue("test");
    console.log(passwordInput.attributes());
    await passwordInput.setValue("");

    await waitForExpect(() => {
      expect(
        wrapper.find('div[label="Encryption password"]').find("span").text()
      ).toEqual("Password is required.");
    });
  });

  test("Date validation", async () => {
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");

    const dateInput = wrapper.find('input[type="date"]');
    await dateInput.setValue("2022-12-01");

    await waitForExpect(() => {
      console.log(dateInput.attributes());
      console.log(
        wrapper.find('div[label="Optional expiry date"]').find("span").text()
      );
      expect(
        wrapper.find('div[label="Optional expiry date"]').find("span").text()
      ).toEqual("Expiry date must be earlier than today.");
    }, 1000);
  });
});
