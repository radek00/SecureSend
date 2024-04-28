<script setup lang="ts">
import FileCard from "@/components/FileCard.vue";
import CopyIcon from "@/assets/icons/CopyIcon.vue";
import DropdownIcon from "@/assets/icons/DropdownIcon.vue";
import { useToggle } from "@/utils/composables/useTogglle";
import type { HistoryItem } from "@/models/HistoryItem";
import { useAlert } from "@/utils/composables/useAlert";

defineProps<{
  upload: HistoryItem;
}>();

const { value, toggle } = useToggle(false);

const { openSuccess } = useAlert();
const copyToClipboard = (link: string) => {
  navigator.clipboard.writeText(link);
  openSuccess("Link copied to clipboard");
};
</script>

<template>
  <li class="py-3 sm:py-4">
    <div class="flex items-center">
      <div class="flex-shrink-0" @click="toggle()">
        <button
          data-test="history-expand"
          class="text-sm underline cursor-pointer text-blue-500 hover:text-blue-300 hover:no-underline flex items-center gap-2 mt-2"
        >
          <DropdownIcon :class="{ 'rotate-180': value }"></DropdownIcon>
        </button>
      </div>
      <div class="flex-1 min-w-0 ms-4">
        <p
          class="text-sm font-medium truncate text-white"
          data-test="history-title"
        >
          {{ new Date(upload.uploadDate).toDateString() }}
        </p>
        <p class="text-sm truncate text-gray-400">
          Expires on {{ new Date(upload.expirationDate).toDateString() }}
        </p>
      </div>
      <div class="inline-flex items-center text-base font-semibold text-white">
        <button
          class="text-gray-400 bg-gray-800 border-gray-600 hover:bg-gray-700 rounded-lg py-2 px-2.5 inline-flex items-center justify-center border"
          @click="copyToClipboard(upload.link)"
        >
          <span id="default-message" class="inline-flex items-center">
            <CopyIcon></CopyIcon>
            <span class="text-xs font-semibold">Copy link</span>
          </span>
        </button>
      </div>
    </div>
    <ul class="ml-[1.6rem] mt-3 flex flex-col gap-3" v-if="value">
      <li v-for="(file, idx) in upload.files" :key="idx">
        <FileCard
          :file-name="file.fileName!"
          :size="file.fileSize"
          class="bg-slate-700"
        ></FileCard>
      </li>
    </ul>
  </li>
</template>

<style scoped></style>
