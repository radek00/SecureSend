<script setup lang="ts">
import HistoryElement from "@/components/UploadHistory/HistoryElement.vue";
import type { HistoryItem } from "@/models/HistoryItem";

defineProps<{
  uploads: HistoryItem[];
}>();
</script>

<template>
  <div class="flex items-center justify-between mb-4" v-if="uploads">
    <h5 class="text-xl font-bold leading-none text-white">Upload history</h5>
  </div>
  <div class="overflow-auto max-h-40 lg:max-h-96 p-2">
    <ul role="list" class="divide-y divide-gray-700">
      <HistoryElement
        v-for="upload in uploads"
        :upload="upload"
      ></HistoryElement>
    </ul>
  </div>
</template>

<style scoped></style>
