<template>
  <button
    type="submit"
    class="text-white focus:ring-4 focus:enabled::outline-none font-medium rounded-lg text-sm w-full sm:w-auto px-5 py-2.5 text-center disabled:cursor-not-allowed disabled:bg-gray-600"
    :class="{
      'dark:bg-blue-600 dark:hover:enabled::bg-blue-700 dark:focus:enabled:ring-blue-800 bg-blue-700 hover:enabled:bg-blue-800 focus:enabled:ring-blue-300':
        type === ButtonType.primary,
      'bg-gray-800 hover:enabled:bg-gray-900 focus:enabled:ring-gray-700':
        type === ButtonType.back,
    }"
  >
    <slot>Submit</slot>
  </button>
</template>

<script setup lang="ts">
import { ButtonType } from "@/models/enums/ButtonType";
const props = defineProps<{
  type: ButtonType;
}>();
</script>
