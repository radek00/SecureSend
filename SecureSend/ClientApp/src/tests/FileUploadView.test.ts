import { VueWrapper, mount, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView.vue";
import { ref } from "vue";
import { waitForExpect } from "@/tests/utils";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import { clickOutside } from "@/utils/composables/directives/clickOutside";
import { SecureSendService } from "@/services/SecureSendService";

describe("FileUploadView", () => {
  let wrapper: VueWrapper<any>;
  beforeEach(async () => {
    const div = document.createElement("div");
    const alertContainer = document.createElement("div");
    alertContainer.id = "alert-container";
    div.id = "root";
    document.body.appendChild(alertContainer);
    document.body.appendChild(div);
    wrapper = mount(FileUploadView, {
      global: {
        provide: { isLoading: ref(false) },
        directives: {
          "click-outside": clickOutside,
        },
      },
    });
    await flushPromises();
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
      expect(
        wrapper.find('div[label="Optional expiry date"]').find("span").text()
      ).toEqual("Expiry date must be earlier than today.");
    });
  });

  test("File input", async () => {
    SecureSendService.createSecureUpload = vi.fn().mockImplementation(() => {});
    SecureSendService.uploadChunk = vi.fn().mockReturnValue({});

    const file = new File(["file content"], "test.txt", { type: "text/plain" });
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");
    await submitButton.trigger("submit");

    await waitForExpect(() => {
      expect(submitButton.text()).toEqual("Upload");
      expect(submitButton.attributes("disabled")).toBeDefined();
    });

    const fileInputComponent = wrapper.findComponent(FileInput);
    fileInputComponent.vm.$emit("onFielsChange", [file]);
    await wrapper.vm.$nextTick();

    expect(wrapper.find("#add-more-files").exists()).toEqual(true);
    expect(submitButton.attributes("disabled")).toBeUndefined();
    expect(fileInputComponent.text()).toContain("0%");
    expect(fileInputComponent.find("button span").text()).toEqual(
      "Remove file"
    );
    await submitButton.trigger("submit");

    await waitForExpect(async () => {
      const closeButton = wrapper.find("#close-modal-button");
      await closeButton.trigger("click");
      expect(document.querySelector("#alert-container")?.innerHTML).toContain(
        "Upload successful"
      );
    });
  });
});
