import { VueWrapper, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent, waitForExpect } from "@/tests/utils";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import { SecureSendService } from "@/services/SecureSendService";
import { UploadState } from "@/models/UploadStateTuple";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import SizeLimit from "@/components/FileUploadForm/SizeLimit.vue";
import UploadHistory from "@/components/UploadHistory/UploadHistory.vue";
import FileCard from "@/components/FileCard.vue";

describe("FileUploadView", () => {
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
    await dateInput.setValue("2021-12-01");
    await waitForExpect(async () => {
      expect(
        wrapper
          .find('div[data-test="expirationDate"] p[data-test="error-message"]')
          .text()
      ).toEqual("Expiry date must be later than today.");
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
    fileInputComponent.vm.$emit("onFilesChange", [file]);
    await wrapper.vm.$nextTick();

    expect(wrapper.find('input[data-test="add-more-files"]').exists()).toEqual(
      true
    );
    expect(submitButton.attributes("disabled")).toBeUndefined();
    expect(wrapper.find('div[data-test="progress-bar"]').text()).toEqual(
      "Upload not started"
    );
    expect(fileInputComponent.find("button span").text()).toEqual(
      "Remove file"
    );
    await submitButton.trigger("submit");

    await waitForExpect(async () => {
      const closeButton = wrapper.find(
        'button[data-test="close-modal-button"]'
      );
      await closeButton.trigger("click");
      expect(document.querySelector("#alert-container")?.innerHTML).toContain(
        "Upload successful"
      );
    });
  });

  test("File status upload handling", async () => {
    const file = new File(["file content"], "test.txt", { type: "text/plain" });
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");
    await submitButton.trigger("submit");

    await waitForExpect(() => {
      expect(submitButton.text()).toEqual("Upload");
      expect(submitButton.attributes("disabled")).toBeDefined();
    });

    const fileInputComponent = wrapper.findComponent(FileInput);
    fileInputComponent.vm.$emit("onFilesChange", [file]);
    wrapper.vm.files.set(file, ["50%", UploadState.InProgress]);
    await wrapper.vm.$nextTick();

    const progressBar = wrapper.find('div[data-test="progress-bar"]');

    expect(wrapper.find('button[data-test="cancel-button"]').exists()).toEqual(
      true
    );
    expect(wrapper.find('button[data-test="pause-button"]').exists()).toEqual(
      true
    );
    expect(progressBar.text()).toEqual("50%");

    wrapper.vm.files.set(file, ["Upload paused", UploadState.Paused]);
    await wrapper.vm.$forceUpdate();

    expect(wrapper.find('button[data-test="resume-button"]').exists()).toEqual(
      true
    );
    expect(wrapper.find('button[data-test="cancel-button"]').exists()).toEqual(
      false
    );
    expect(wrapper.find('button[data-test="pause-button"]').exists()).toEqual(
      false
    );
    expect(progressBar.text()).toEqual("Upload paused");

    wrapper.vm.files.set(file, ["Finishing upload...", UploadState.Merging]);
    await wrapper.vm.$forceUpdate();
    expect(progressBar.text()).toEqual("Finishing upload...");
    expect(wrapper.findComponent(LoadingIndicator).exists()).toBe(true);

    wrapper.vm.files.set(file, ["Upload completed", UploadState.Completed]);
    await wrapper.vm.$forceUpdate();
    expect(progressBar.text()).toEqual("Upload completed");
    expect(wrapper.findComponent(LoadingIndicator).exists()).toBe(false);
    expect(wrapper.find('button[data-test="resume-button"]').exists()).toEqual(
      false
    );
    expect(wrapper.find('button[data-test="cancel-button"]').exists()).toEqual(
      false
    );
    expect(wrapper.find('button[data-test="pause-button"]').exists()).toEqual(
      false
    );

    wrapper.vm.files.set(file, ["Upload failed", UploadState.Failed]);
    await wrapper.vm.$forceUpdate();
    expect(progressBar.text()).toEqual("Upload failed");
    expect(wrapper.findComponent(LoadingIndicator).exists()).toBe(false);
    expect(wrapper.find('button[data-test="resume-button"]').exists()).toEqual(
      false
    );
    expect(wrapper.find('button[data-test="cancel-button"]').exists()).toEqual(
      false
    );
    expect(wrapper.find('button[data-test="pause-button"]').exists()).toEqual(
      false
    );
  });
});

describe("Form validation when size limits are set", () => {
  //vi.useFakeTimers();
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

  test("History item gets added after successful or partially successful upload", async () => {
    SecureSendService.createSecureUpload = vi.fn().mockImplementation(() => {});
    SecureSendService.uploadChunk = vi.fn().mockReturnValue({});

    const file = new File(["file content"], "test.txt", { type: "text/plain" });
    const submitButton = wrapper.find('button[type="submit"]');

    await submitButton.trigger("submit");
    await submitButton.trigger("submit");

    const fileInputComponent = wrapper.findComponent(FileInput);
    fileInputComponent.vm.$emit("onFilesChange", [file]);
    await wrapper.vm.$nextTick();

    await submitButton.trigger("submit");

    await waitForExpect(async () => {
      const closeButton = wrapper.find(
        'button[data-test="close-modal-button"]'
      );
      await closeButton.trigger("click");
    });

    //check if item gets added
    const historyComponent = wrapper.findComponent(UploadHistory);
    expect(historyComponent.exists()).toBe(true);
    expect(
      historyComponent.find(`p[data-test="history-title"]`).text()
    ).toEqual("Sun Feb 20 2022");

    //check if list is collapsed
    const expandButton = historyComponent.find(
      `button[data-test="history-expand"]`
    );
    expect(historyComponent.findComponent(FileCard).exists()).toBe(false);

    //check if correct file card appears after expanding
    await expandButton.trigger("click");
    const fileCard = historyComponent.findComponent(FileCard);
    expect(fileCard.exists()).toBe(true);
    expect(fileCard.text()).toContain("test.txt");
  });
});
