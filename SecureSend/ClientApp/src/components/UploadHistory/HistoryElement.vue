<script setup lang="ts">
import FileCard from "@/components/FileCard.vue";
import CopyIcon from "@/assets/icons/CopyIcon.vue";
import DropdownIcon from "@/assets/icons/DropdownIcon.vue";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { useToggle } from "@/utils/composables/useTogglle";

const props = defineProps<{
  upload?: SecureUploadDto;
}>();

const { value, toggle } = useToggle(false);
</script>

<template>
  <li class="py-3 sm:py-4">
    <div class="flex items-center">
      <div class="flex-shrink-0" @click="toggle()">
        <a
          class="text-sm underline cursor-pointer text-blue-500 hover:no-underline flex items-center gap-2 mt-2"
        >
          <DropdownIcon :class="{ 'rotate-180': value }"></DropdownIcon>
        </a>
      </div>
      <div class="flex-1 min-w-0 ms-4">
        <p class="text-sm font-medium truncate text-white">23.04.2024</p>
        <p class="text-sm truncate text-gray-400">5 files</p>
      </div>
      <div class="inline-flex items-center text-base font-semibold text-white">
        <button
          class="text-gray-400 bg-gray-800 border-gray-600 hover:bg-gray-700 rounded-lg py-2 px-2.5 inline-flex items-center justify-center border"
        >
          <span id="default-message" class="inline-flex items-center">
            <CopyIcon></CopyIcon>
            <span class="text-xs font-semibold">Copy</span>
          </span>
        </button>
      </div>
    </div>
    <ul class="ml-[1.6rem] mt-3 flex flex-col gap-3" v-if="value">
      <li>
        <FileCard
          file-name="hello.exe"
          :size="5000"
          class="bg-slate-700"
        ></FileCard>
      </li>
      <li>
        <FileCard
          file-name="hello.exe"
          :size="5000"
          class="bg-slate-700"
        ></FileCard>
      </li>
      <li>
        <FileCard
          file-name="hello.exe"
          :size="5000"
          class="bg-slate-700"
        ></FileCard>
      </li>
    </ul>
  </li>
</template>

<style scoped></style>
