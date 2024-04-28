import { VueWrapper, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent, waitForExpect } from "@/tests/utils";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import { SecureSendService } from "@/services/SecureSendService";
import { UploadState } from "@/models/UploadStateTuple";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import { UploadLimitsService } from "@/services/UploadLimitsService";

describe("File input", () => {
  let wrapper: VueWrapper<any>;
  beforeEach(async () => {
    vi.setSystemTime(new Date(2022, 1, 20));
    UploadLimitsService.getUploadLimits = vi.fn().mockReturnValue({});
    wrapper = mountComponent(FileUploadView).wrapper;
    await flushPromises();
    await wrapper.vm.$nextTick();
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
