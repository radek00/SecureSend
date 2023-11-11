import "./assets/main.css";

import { createApp, ref } from "vue";
import App from "./App.vue";
import router from "./router";
import { useAlert } from "./utils/composables/useAlert";

const app = createApp(App);

const { openDanger } = useAlert();

const isLoading = ref<boolean>(false);

app.provide("isLoading", isLoading);

app.config.errorHandler = (err) => {
  console.error(err);
  isLoading!.value = false;
  openDanger("Something went wrong");
};

const registerServiceWorker = async () => {
  if ("serviceWorker" in navigator) {
    try {
      const registration = await navigator.serviceWorker.register(
        `${import.meta.env.BASE_URL}${import.meta.env.VITE_WORKER}`,
        { type: "module", scope: "/" }
      );
      const devMode = import.meta.env.DEV;
      if (devMode) {
        if (registration.installing) {
          console.log("Service worker installing");
        } else if (registration.waiting) {
          console.log("Service worker installed");
        } else if (registration.active) {
          console.log("Service worker active");
        }
      }
    } catch (error) {
      console.error(`Registration failed with ${error}`);
    }
  }
};
registerServiceWorker();

app.use(router);

app.mount("#app");
