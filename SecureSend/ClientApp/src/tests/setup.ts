import { vi } from "vitest";
import { ref } from "vue";

// Mock useScreenSize composable globally for all tests (mobile by default)
vi.mock("@/utils/composables/useScreenSize", async (importOriginal) => {
  const actual =
    await importOriginal<typeof import("@/utils/composables/useScreenSize")>();
  return {
    ...actual,
    useScreenSize: () => ({
      screenType: ref(actual.ScreenType.XS), // Mobile screen by default
      isDesktop: () => false,
    }),
  };
});
