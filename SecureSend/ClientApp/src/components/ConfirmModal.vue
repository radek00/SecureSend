<script setup lang="ts">
import CloseIcon from "@/assets/icons/CloseIcon.vue";
import { onMounted, ref } from "vue";
defineEmits(["closeClick"]);

const dialogElement = ref<HTMLElement>();

onMounted(() => {
  trapFocus(dialogElement.value!);
});
const trapFocus = (element: HTMLElement) => {
  const focusableEls = element.querySelectorAll<HTMLElement>(
    'a[href]:not([disabled]), button:not([disabled]), textarea:not([disabled]), input[type="text"]:not([disabled]), input[type="radio"]:not([disabled]), input[type="checkbox"]:not([disabled]), select:not([disabled])'
  );
  console.log(focusableEls);
  const firstFocusableEl = focusableEls[0];
  const lastFocusableEl = focusableEls[focusableEls.length - 1];
  const KEYCODE_TAB = "9";

  element.addEventListener("keydown", function (e) {
    const isTabPressed = e.key === "Tab" || e.code === KEYCODE_TAB;
    console.log("key clicked");
    if (!isTabPressed) {
      return;
    }

    if (e.shiftKey) {
      if (document.activeElement === firstFocusableEl) {
        lastFocusableEl.focus();
        e.preventDefault();
      }
    } else {
      if (document.activeElement === lastFocusableEl) {
        firstFocusableEl.focus();
        e.preventDefault();
      }
    }
  });
};
</script>

<template>
  <!-- Main modal -->
  <div
    role="dialog"
    aria-modal="true"
    class="backdrop-blur fixed top-0 bottom-1/2 left-0 z-40 h-full w-screen flex items-center justify-center p-5 bg-gray-900/50"
  >
    <div class="relative w-full max-w-2xl max-h-full" ref="dialogElement">
      <!-- Modal content -->
      <div class="relative rounded-lg shadow bg-gray-700">
        <!-- Modal header -->
        <div
          class="flex items-start justify-between p-4 border-b rounded-t border-gray-600"
        >
          <h3 class="text-xl font-semibold text-white">
            <slot name="header"></slot>
          </h3>
          <button
            data-test="close-modal-button"
            @click="$emit('closeClick')"
            type="button"
            class="text-gray-400 bg-transparent rounded-lg text-sm w-8 h-8 ml-auto inline-flex justify-center items-center hover:bg-gray-600 hover:text-white"
          >
            <CloseIcon></CloseIcon>
            <span class="sr-only">Close modal</span>
          </button>
        </div>
        <!-- Modal body -->

        <div
          class="p-6 space-y-6 overflow-x-hidden overflow-y-auto max-h-[200px]"
        >
          <p class="text-base leading-relaxed text-gray-400">
            <slot name="body"></slot>
          </p>
        </div>

        <!-- Modal footer -->
        <div
          class="flex items-center p-4 space-x-2 border-t rounded-b border-gray-600"
        >
          <slot name="footer"></slot>
        </div>
      </div>
    </div>
  </div>
</template>
