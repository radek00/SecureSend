<template>
  <div
    class="w-auto p-6 bg-white border border-gray-200 rounded-lg shadow dark:bg-gray-800 dark:border-gray-700"
  >
    <div class="flex w-full gap-4 items-center">
      <div>
        <FileIcon></FileIcon>
      </div>
      <div class="flex-1 min-w-0">
        <div class="flex justify-between gap-2">
          <div class="flex-1 min-w-0">
            <p class="truncate">{{ fileName }}</p>
          </div>
          <p v-if="size">{{ fileSize(size) }}</p>
        </div>

        <slot name="cardBottom"> </slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { fileSize } from "@/utils/utils";
import FileIcon from "@/assets/icons/FileIcon.vue";
defineProps<{
  fileName: string;
  size?: number;
}>();
</script>
