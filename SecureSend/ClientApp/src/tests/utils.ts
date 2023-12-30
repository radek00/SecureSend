import {mount} from "@vue/test-utils";
import { ref } from "vue";
import type {Component} from "vue";
import {clickOutside} from "@/utils/composables/directives/clickOutside";

export const waitForExpect = async (callback: () => any, waitFor = 100) => {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      try {
        resolve(callback());
      } catch (err) {
        reject(err);
      }
    }, waitFor);
  });
};

export const mountComponent = (component: Component) => {
  const div = document.createElement("div");
  const alertContainer = document.createElement("div");
  alertContainer.id = "alert-container";
  div.id = "root";
  document.body.appendChild(alertContainer);
  document.body.appendChild(div);
  return mount(component, {
    global: {
      provide: { isLoading: ref(false) },
      directives: {
        "click-outside": clickOutside,
      },
    },
  });
}
