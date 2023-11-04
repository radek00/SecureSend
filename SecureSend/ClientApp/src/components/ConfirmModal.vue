<script setup lang="ts">
import CloseIcon from "@/assets/icons/CloseIcon.vue";
defineEmits(["closeClick"]);
</script>

<template>
  <!-- Main modal -->
  <div
    tabindex="-1"
    aria-hidden="true"
    class="backdrop-blur fixed top-0 bottom-1/2 left-0 z-40 h-screen w-screen flex items-center justify-center bg-gray-900/50"
  >
    <div class="relative w-full max-w-2xl max-h-full">
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
