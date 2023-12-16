import { VueWrapper, mount, flushPromises } from "@vue/test-utils";
import { beforeEach, describe, expect, test, vi } from "vitest";
import FileUploadView from "@/views/FileUploadView.vue";
import { ref } from "vue";

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

  test("No password form flow", async () => {
    let submitButton = wrapper.find('button[type="submit"]');
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
    setTimeout(() => {
      expect(submitButton.text()).toEqual("Upload");
    }, 1000);

    await submitButton.trigger("submit");
    await flushPromises();
    //setTimeout(())
  });
});
