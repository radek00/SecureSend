<template>
  <div class="w-auto p-6 rounded-lg shadow bg-gray-800 border-gray-700">
    <div class="flex w-full gap-4 items-center">
      <div class="hidden sm:block">
        <FileIcon></FileIcon>
      </div>
      <div class="flex-1 min-w-0">
        <div class="flex justify-between gap-2 items-center flex-wrap">
          <div class="w-full sm:flex-1 sm:w-auto md:min-w-0">
            <p class="truncate">{{ fileName }}</p>
          </div>
          <p v-if="size">{{ fileSize(size) }}</p>
          <slot name="cardMiddle"></slot>
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
