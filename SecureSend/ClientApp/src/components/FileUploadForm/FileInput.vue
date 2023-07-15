<script setup lang="ts">
import UploadIcon from "@/assets/icons/UploadIcon.vue";
import FileCard from "../FileCard.vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import CheckIcon from "@/assets/icons/CheckIcon.vue";

defineEmits(["onFielsChange"]);

defineProps<{
  files: Map<File, number | string | boolean>;
}>();
</script>

<template>
  <div v-if="!files.size" class="flex items-center justify-center w-full">
    <label
      for="dropzone-file"
      class="flex flex-col items-center justify-center w-full h-64 border-2 border-gray-300 border-dashed rounded-lg cursor-pointer bg-gray-50 dark:hover:bg-bray-800 dark:bg-gray-700 hover:bg-gray-100 dark:border-gray-600 dark:hover:border-gray-500 dark:hover:bg-gray-600"
    >
      <div class="flex flex-col items-center justify-center pt-5 pb-6">
        <UploadIcon></UploadIcon>
        <p class="mb-2 text-sm text-gray-500 dark:text-gray-400">
          <span class="font-semibold">Click to upload</span> or drag and drop
        </p>
      </div>
      <input
        @change="$emit('onFielsChange', $event)"
        id="dropzone-file"
        type="file"
        class="hidden"
        multiple
      />
    </label>
  </div>

  <div
    v-else
    class="flex flex-col gap-5 mt-5 w-full justify-between max-h-[300px] overflow-y-auto p-6 border border-gray-300 rounded-lg shadow dark:bg-gray-700 dark:border-gray-600"
  >
    <FileCard
      v-for="[key, value] in files"
      :file-name="key.name"
      :size="key.size"
    >
      <template #cardMiddle>
        <LoadingIndicator
          v-if="value === 100"
          class="w-8 h-5 mr-2"
        ></LoadingIndicator>
        <CheckIcon v-if="value === true"></CheckIcon>
      </template>
      <template #cardBottom>
        <div class="w-full bg-gray-200 rounded-full dark:bg-gray-700 mt-2">
          <div
            class="bg-blue-600 text-xs font-medium text-blue-100 text-center p-0.5 leading-none rounded-full"
            :style="{
              width: `${
                value === true ? 100 : typeof value === 'number' ? value : 100
              }%`,
            }"
          >
            {{
              typeof value === "string"
                ? value
                : typeof value === "number"
                ? value === 100
                  ? "Finishing upload..."
                  : `${value}%`
                : "Upload completed"
            }}
          </div>
        </div>
      </template>
    </FileCard>
  </div>
</template>
