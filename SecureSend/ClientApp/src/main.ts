import "./assets/main.css";

import { createApp, ref } from "vue";
import App from "./App.vue";
import router from "./router";
import { useAlert } from "./utils/composables/useAlert";
import { clickOutside } from "@/utils/composables/directives/clickOutside";

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

app.use(router);

app.mount("#app");
