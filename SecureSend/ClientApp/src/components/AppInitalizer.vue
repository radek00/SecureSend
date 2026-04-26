<script setup lang="ts">
import router from "@/router";
import { debugLog } from "@/utils/utils";
import { ref } from "vue";

const isWorkerLoading = ref<boolean>(true);

const registerServiceWorker = async () => {
  if ("serviceWorker" in navigator) {
    try {
      const registration = await navigator.serviceWorker.register(
        `${import.meta.env.BASE_URL}${import.meta.env.VITE_WORKER}`,
        { type: "module", scope: "/" }
      );
      if (registration.installing) {
        debugLog("Service worker installing");
      } else if (registration.waiting) {
        debugLog("Service worker installed");
      } else if (registration.active) {
        debugLog("Service worker active");
      }

      await navigator.serviceWorker.ready;
      isWorkerLoading.value = false;
    } catch (error) {
      console.error(`Registration failed with ${error}`);
      isWorkerLoading.value = false;
      router.push({ name: "error", params: { errorCode: "unsupported" } });
    }
  } else {
    isWorkerLoading.value = false;
    router.push({ name: "error", params: { errorCode: "unsupported" } });
  }
};
registerServiceWorker();
</script>

<template>
  <p
    v-if="isWorkerLoading"
    class="flex h-full items-center justify-center text-4xl"
  >
    Loading...
  </p>
  <slot v-else></slot>
</template>
