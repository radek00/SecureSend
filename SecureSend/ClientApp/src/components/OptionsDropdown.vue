<template>
  <button
    @click="isExpanded = !isExpanded"
    id="dropdownMenuIconHorizontalButton"
    data-dropdown-toggle="dropdownDotsHorizontal"
    class="relative inline-flex items-center p-2 text-sm font-medium text-center text-gray-900 bg-white rounded-lg hover:bg-gray-100 focus:ring-4 focus:outline-none dark:text-white focus:ring-gray-50 dark:bg-gray-800 dark:hover:bg-gray-700 dark:focus:ring-gray-600"
    type="button"
  >
    <svg
      class="w-5 h-5"
      aria-hidden="true"
      xmlns="http://www.w3.org/2000/svg"
      fill="currentColor"
      viewBox="0 0 16 3"
    >
      <path
        d="M2 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3Zm6.041 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3ZM14 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3Z"
      />
    </svg>
  </button>
  <!-- Dropdown menu -->
  <div
    v-if="isExpanded"
    id="dropdown"
    class="z-10 absolute divide-y divide-gray-100 rounded-lg shadow w-auto bg-gray-900 -translate-x-10"
  >
    <ul
      class="py-2 text-sm text-gray-200"
      aria-labelledby="dropdownDefaultButton"
    >
      <slot></slot>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";

const isExpanded = ref<boolean>(false);
</script>
