<template>
  <div class="relative" v-click-outside="() => (isExpanded = false)">
    <button
      @click="isExpanded = !isExpanded"
      class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300 focus:outline-none focus:ring-1 focus:ring-gray-400 dark:focus:ring-gray-400 rounded-full"
      type="button"
    >
      <span class="sr-only">Show information about password functionality</span>
      <QuestionCircle class="w-5 h-5"></QuestionCircle>
    </button>
    <div
      v-if="isExpanded"
      role="tooltip"
      v-bind="$attrs"
      class="absolute p-3 text-sm font-light text-gray-600 bg-gray-50 border border-gray-200 rounded-lg shadow-sm dark:text-gray-400 dark:border-gray-600 dark:bg-gray-900 bottom-[65%] mb-2 w-64 left-1/2 transform -translate-x-1/2"
    >
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, useId } from "vue";
import QuestionCircle from "@/assets/icons/QuestionCircle.vue";

const isExpanded = ref<boolean>(false);
</script>
