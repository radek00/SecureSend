<template>
  <button
    type="submit"
    class="text-white focus:ring-4 focus:enabled::outline-none font-medium rounded-lg text-sm px-5 py-2.5 text-center disabled:cursor-not-allowed disabled:bg-gray-700"
    :class="{
      'bg-blue-600 hover:enabled::bg-blue-700 focus:enabled:ring-blue-800 hover:enabled:bg-blue-800':
        type === ButtonType.primary,
      'bg-gray-800 hover:enabled:bg-gray-900 focus:enabled:ring-gray-700':
        type === ButtonType.back,
      'bg-red-600 hover:bg-red-700 focus:ring-red-900':
        type === ButtonType.cancel,
    }"
  >
    <slot>Submit</slot>
  </button>
</template>

<script setup lang="ts">
import { ButtonType } from "@/models/enums/ButtonType";

defineProps<{
  type: ButtonType;
}>();
</script>
