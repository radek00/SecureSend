import { ref, onMounted, onUnmounted } from "vue";

export enum ScreenType {
  XS = "xs",
  SM = "sm",
  MD = "md",
  LG = "lg",
  XL = "xl",
  XXL = "2xl",
}

const breakpoints = {
  sm: 640,
  md: 768,
  lg: 1024,
  xl: 1280,
  "2xl": 1536,
} as const;

export function isDesktop(screenType: ScreenType) {
  return (
    screenType === ScreenType.LG ||
    screenType === ScreenType.XL ||
    screenType === ScreenType.XXL
  );
}

export function useScreenSize() {
  const getScreenType = (width: number): ScreenType => {
    if (width >= breakpoints["2xl"]) return ScreenType.XXL;
    if (width >= breakpoints.xl) return ScreenType.XL;
    if (width >= breakpoints.lg) return ScreenType.LG;
    if (width >= breakpoints.md) return ScreenType.MD;
    if (width >= breakpoints.sm) return ScreenType.SM;
    return ScreenType.XS;
  };

  const screenType = ref<ScreenType>(getScreenType(window.innerWidth));

  const handleResize = () => {
    screenType.value = getScreenType(window.innerWidth);
  };

  onMounted(() => {
    window.addEventListener("resize", handleResize);
  });

  onUnmounted(() => {
    window.removeEventListener("resize", handleResize);
  });

  return {
    screenType,
  };
}
