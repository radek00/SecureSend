<template>
  <button
    class="text-white focus:ring-4 focus:enabled::outline-none font-medium rounded-lg text-sm px-5 py-2.5 text-center disabled:cursor-not-allowed disabled:bg-gray-300 dark:disabled:bg-gray-700 disabled:text-gray-500"
    :class="{
      'bg-blue-600 hover:enabled::bg-blue-700 focus:enabled:ring-blue-300 dark:focus:enabled:ring-blue-800':
        category === ButtonType.primary,
      'bg-gray-600 hover:enabled:bg-gray-700 focus:enabled:ring-gray-300 dark:bg-gray-800 dark:hover:enabled:bg-gray-900 dark:focus:enabled:ring-gray-700':
        category === ButtonType.back,
      'bg-red-600 hover:bg-red-700 focus:ring-red-300 dark:focus:ring-red-900':
        category === ButtonType.cancel,
    }"
  >
    <slot>Submit</slot>
  </button>
</template>

<script setup lang="ts">
import { ButtonType } from "@/models/enums/ButtonType";

defineProps<{
  category: ButtonType;
}>();
</script>
