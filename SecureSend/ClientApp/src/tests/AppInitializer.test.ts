import { flushPromises, mount } from "@vue/test-utils";
import { describe, test, expect, vi, afterEach } from "vitest";
import AppInitalizer from "@/components/AppInitializer.vue";
import router from "@/router";

vi.mock("@/router", () => ({
  default: {
    push: vi.fn(),
  },
}));

describe("AppInitalizer", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  const mountSubject = () => {
    return mount(AppInitalizer, {
      slots: {
        default: '<div class="slotted-content">App Content</div>',
      },
    });
  };

  test("renders loading initially and shows slot on success", async () => {
    vi.stubGlobal("navigator", {
      serviceWorker: {
        register: vi.fn().mockResolvedValue({ active: true }),
        ready: Promise.resolve(),
      },
    });

    const wrapper = mountSubject();

    expect(wrapper.text()).toContain("Loading...");

    await flushPromises();

    expect(wrapper.text()).not.toContain("Loading...");
    expect(wrapper.html()).toContain("slotted-content");
  });

  test("redirects to error page if serviceWorker registration fails", async () => {
    vi.stubGlobal("navigator", {
      serviceWorker: {
        register: vi.fn().mockRejectedValue(new Error("Test failure")),
        ready: Promise.resolve(),
      },
    });

    const wrapper = mountSubject();
    await flushPromises();

    expect(wrapper.text()).not.toContain("Loading...");
    expect(router.push).toHaveBeenCalledWith({
      name: "error",
      params: { errorCode: "unsupported" },
    });
  });

  test("redirects to error page if service worker is completely unsupported", async () => {
    vi.stubGlobal("navigator", {
      serviceWorker: undefined,
    });

    const wrapper = mountSubject();
    await flushPromises();

    expect(wrapper.text()).not.toContain("Loading...");
    expect(router.push).toHaveBeenCalledWith({
      name: "error",
      params: { errorCode: "unsupported" },
    });
  });
});
