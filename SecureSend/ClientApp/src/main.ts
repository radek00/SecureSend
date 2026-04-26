import "./assets/main.css";

import { createApp, ref } from "vue";
import App from "./App.vue";
import router from "./router";
import { useAlert } from "./utils/composables/useAlert";
import { clickOutside } from "@/utils/composables/directives/clickOutside";
import { debugLog } from "./utils/utils";

const app = createApp(App);

const { openDanger } = useAlert();

const isLoading = ref<boolean>(false);
const isWorkerLoading = ref<boolean>(true);

app.provide("isLoading", isLoading);
app.provide("isWorkerLoading", isWorkerLoading);

app.config.errorHandler = (err) => {
  console.error(err);
  isLoading!.value = false;
  isWorkerLoading.value = false;
  openDanger("Something went wrong");
};

app.directive("click-outside", clickOutside);

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

app.use(router);
registerServiceWorker();

app.mount("#app");
