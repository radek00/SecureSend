import { VueWrapper } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView/FileUploadView.vue";
import { mountComponent, waitForExpect } from "@/tests/utils";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import { SecureSendService } from "@/services/SecureSendService";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import UploadHistory from "@/components/UploadHistory/UploadHistory.vue";
import FileCard from "@/components/FileCard.vue";

async function prepareForHistoryTest(wrapper: VueWrapper<any>) {
  const file = new File(["file content"], "test.txt", { type: "text/plain" });
  const file2 = new File(["file content2"], "test2.txt", {
    type: "text/plain",
  });
  const submitButton = wrapper.find('button[type="submit"]');

  await submitButton.trigger("submit");
  await submitButton.trigger("submit");

  const fileInputComponent = wrapper.findComponent(FileInput);
  fileInputComponent.vm.$emit("onFilesChange", [file, file2]);
  await wrapper.vm.$nextTick();

  await submitButton.trigger("submit");

  await waitForExpect(async () => {
    const closeButton = wrapper.find('button[data-test="close-modal-button"]');
    await closeButton.trigger("click");
  });

  return wrapper.findComponent(UploadHistory);
}
describe("Upload history", () => {
  //vi.useFakeTimers();
  beforeEach(async () => {
    vi.setSystemTime(new Date(2022, 1, 20));
    UploadLimitsService.getUploadLimits = vi.fn().mockReturnValue({});
    SecureSendService.createSecureUpload = vi.fn().mockImplementation(() => {});
    SecureSendService.uploadChunk = vi.fn().mockReturnValue({});
  });

  test("History item gets added after successful or partially successful upload", async () => {
    const wrapper = mountComponent(FileUploadView).wrapper;
    const historyComponent = await prepareForHistoryTest(wrapper);
    expect(historyComponent.exists()).toBe(true);

    //check if upload gets added to history
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

  test("Expired uploads are removed from history", async () => {
    let wrapper = mountComponent(FileUploadView).wrapper;

    let historyComponent = await prepareForHistoryTest(wrapper);
    expect(historyComponent.exists()).toBe(true);
    wrapper.unmount();
    vi.setSystemTime(new Date(2022, 2, 20));
    wrapper = mountComponent(FileUploadView).wrapper;

    //check if expired upload gets removed from history
    historyComponent = wrapper.findComponent(UploadHistory);
    expect(historyComponent.exists()).toBe(false);
  });
});
