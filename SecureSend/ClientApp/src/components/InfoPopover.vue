<template>
  <button
    :style="{ 'anchor-name': `--${anchorId}` }"
    :popovertarget="popoverId"
    class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300 focus:outline-none focus:ring-1 focus:ring-gray-200 dark:focus:ring-gray-400 rounded-full"
    type="button"
  >
    <QuestionCircle class="w-5 h-5"></QuestionCircle>
  </button>
  <div
    :id="popoverId"
    popover
    :style="{
      'position-anchor': `--${anchorId}`,
      margin: 0,
      inset: 'auto',
      bottom: 'anchor(top)',
      left: 'anchor(center)',
      transform: 'translateY(-8px) translateX(-50%)',
    }"
    class="p-3 text-sm font-light text-gray-500 bg-white border border-gray-200 rounded-lg shadow-sm dark:text-gray-400 dark:border-gray-600 dark:bg-gray-900 w-64"
  >
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { useId } from "vue";
import QuestionCircle from "@/assets/icons/QuestionCircle.vue";

const baseId = useId().replace(/[^a-zA-Z0-9-]/g, "");
const anchorId = `anchor-${baseId}`;
const popoverId = `popover-${baseId}`;
</script>
