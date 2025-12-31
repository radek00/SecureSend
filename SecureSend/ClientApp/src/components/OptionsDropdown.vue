<template>
  <div class="relative" v-click-outside="() => (isExpanded = false)">
    <button
      @click="isExpanded = !isExpanded"
      class="inline-flex items-center p-2 text-sm font-medium text-center rounded-lg focus:ring-4 focus:outline-hidden text-gray-900 dark:text-white focus:ring-gray-200 dark:focus:ring-gray-50 bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700"
      type="button"
    >
      <ExpandIcon></ExpandIcon>
    </button>
    <div
      v-if="isExpanded"
      id="dropdown"
      class="absolute z-10 divide-y divide-gray-100 rounded-lg shadow-sm w-auto bg-white dark:bg-gray-900"
    >
      <ul
        class="py-2 text-sm text-gray-700 dark:text-gray-200"
        aria-labelledby="dropdownDefaultButton"
      >
        <slot></slot>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import ExpandIcon from "@/assets/icons/ExpandIcon.vue";

const isExpanded = ref<boolean>(false);
</script>
