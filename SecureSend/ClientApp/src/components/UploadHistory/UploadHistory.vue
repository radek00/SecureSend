<script setup lang="ts">
import HistoryElement from "@/components/UploadHistory/HistoryElement.vue";
import type { HistoryItem } from "@/models/HistoryItem";

defineProps<{
  uploads: HistoryItem[];
}>();
</script>

<template>
  <div class="overflow-auto max-h-40 lg:max-h-76 pr-4">
    <ul role="list" class="divide-y divide-gray-200 dark:divide-gray-700">
      <HistoryElement
        v-for="(upload, idx) in uploads"
        :key="idx"
        :upload="upload"
      ></HistoryElement>
    </ul>
  </div>
</template>

<style scoped></style>
