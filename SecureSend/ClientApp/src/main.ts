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

app.provide("isLoading", isLoading);

app.config.errorHandler = (err) => {
  console.error(err);
  isLoading!.value = false;
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
    } catch (error) {
      console.error(`Registration failed with ${error}`);
    }
  }
};
registerServiceWorker();

app.use(router);

app.mount("#app");
