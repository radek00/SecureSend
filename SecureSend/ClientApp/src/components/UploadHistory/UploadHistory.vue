<script setup lang="ts">
import DropdownIcon from "@/assets/icons/DropdownIcon.vue";
import CopyIcon from "@/assets/icons/CopyIcon.vue";
import FileCard from "@/components/FileCard.vue";
import HistoryElement from "@/components/UploadHistory/HistoryElement.vue";
</script>

<template>
  <div class="flex items-center justify-between mb-4">
    <h5 class="text-xl font-bold leading-none text-white">Upload history</h5>
  </div>
  <div class="overflow-auto max-h-40 lg:max-h-96 p-2">
    <ul role="list" class="divide-y divide-gray-700">
      <HistoryElement upload="{}"></HistoryElement>
      <HistoryElement upload="{}"></HistoryElement>
      <HistoryElement upload="{}"></HistoryElement>
      <HistoryElement upload="{}"></HistoryElement>
    </ul>
  </div>
</template>

<style scoped></style>
