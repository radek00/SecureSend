<template>
  <button
    @click="isExpanded = !isExpanded"
    class="inline-flex items-center p-2 text-sm font-medium text-center rounded-lg focus:ring-4 focus:outline-none text-white focus:ring-gray-50 bg-gray-800 hover:bg-gray-700"
    type="button"
  >
    <ExpandIcon></ExpandIcon>
  </button>
  <div
    v-if="isExpanded"
    id="dropdown"
    class="z-10 absolute divide-y divide-gray-100 rounded-lg shadow w-auto bg-gray-900"
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
import ExpandIcon from "@/assets/icons/ExpandIcon.vue";

const isExpanded = ref<boolean>(false);
</script>
