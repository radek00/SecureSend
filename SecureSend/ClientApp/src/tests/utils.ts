import { mount } from "@vue/test-utils";
import { ref } from "vue";
import type { Component } from "vue";
import { clickOutside } from "@/utils/composables/directives/clickOutside";
import { routes } from "@/router";
import { createRouter, createWebHistory } from "vue-router";

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

export const mountComponent = (component: Component, props = {}) => {
  const router = createRouter({
    history: createWebHistory(),
    routes: routes,
  });
  const div = document.createElement("div");
  const alertContainer = document.createElement("div");
  alertContainer.id = "alert-container";
  div.id = "root";
  document.body.appendChild(alertContainer);
  document.body.appendChild(div);
  return {
    wrapper: mount(component, {
      global: {
        provide: { isLoading: ref(false) },
        directives: {
          "click-outside": clickOutside,
        },
        plugins: [router],
      },
      props: props,
    }),
    router,
  };
};
