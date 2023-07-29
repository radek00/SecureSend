<script setup lang="ts">
import UploadIcon from "@/assets/icons/UploadIcon.vue";
import FileCard from "../FileCard.vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import CheckIcon from "@/assets/icons/CheckIcon.vue";
import { ref } from "vue";
import { useDropZone } from "@/utils/composables/useDropZone";
import TrashIcon from "@/assets/icons/TrashIcon.vue";
import PlusIcon from "@/assets/icons/PlusIcon.vue";

const emit = defineEmits<{
  onFielsChange: [files: File[] | null];
  onFileRemove: [file: File];
}>();

defineProps<{
  files: Map<File, number | string | boolean>;
}>();

const fileDropZone = ref<HTMLElement>();

const onDrop = (files: File[] | null) => {
  emit("onFielsChange", files);
};

const { isOverDropZone } = useDropZone(fileDropZone, { onDrop });
</script>

<template>
  <div
    v-if="!files.size"
    class="flex items-center justify-center w-full"
    ref="fileDropZone"
  >
    <label
      for="dropzone-file"
      class="flex flex-col items-center justify-center w-full h-64 border-2 border-gray-300 border-dashed rounded-lg cursor-pointer bg-gray-50 dark:hover:bg-bray-800 dark:bg-gray-700 hover:bg-gray-100 dark:border-gray-600 dark:hover:border-gray-500 dark:hover:bg-gray-600"
    >
      <div class="flex flex-col items-center justify-center pt-5 pb-6">
        <UploadIcon></UploadIcon>
        <p v-if="!isOverDropZone" class="mb-2 text-sm text-gray-400">
          <span class="font-semibold">Click to upload</span> or drag and drop
        </p>
        <p v-else class="mb-2 text-sm text-gray-400">
          <span class="font-semibold">Drop files to upload</span>
        </p>
      </div>
      <input
        @change="
          $emit('onFielsChange', ($event.target as HTMLInputElement).files)
        "
        id="dropzone-file"
        type="file"
        class="hidden"
        multiple
      />
    </label>
  </div>

  <div
    v-else
    class="flex flex-col gap-5 w-full justify-between h-[300px] overflow-y-auto p-6 border border-gray-300 rounded-lg shadow dark:bg-gray-700 dark:border-gray-600"
  >
    <FileCard
      v-for="([key, value], idx) in files"
      :key="idx"
      :file-name="key.name"
      :size="key.size"
    >
      <template #cardMiddle>
        <button
          @click="emit('onFileRemove', key)"
          type="button"
          class="m-0 text-blue-700 border border-red-700 hover:bg-red-700 hover:text-white focus:ring-4 focus:outline-none focus:ring-red-300 font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 dark:border-red-500 dark:text-red-500 dark:hover:text-white dark:focus:ring-red-800 dark:hover:bg-red-500"
        >
          <TrashIcon class="w-5 h-3"></TrashIcon>
          <span class="sr-only">Remove file</span>
        </button>

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
    <label
      for="add-more-files"
      type="button"
      class="w-[fit-content] text-blue-700 border border-blue-700 hover:bg-blue-700 hover:text-white focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-full text-sm p-2.5 text-center inline-flex items-center dark:border-blue-500 dark:text-blue-500 dark:hover:text-white dark:focus:ring-blue-800 dark:hover:bg-blue-500"
    >
      <PlusIcon class="w-4 h-4"></PlusIcon>
      <span class="ml-2">Add more files</span>
    </label>
    <input
      @change="
        $emit('onFielsChange', ($event.target as HTMLInputElement).files)
      "
      id="add-more-files"
      type="file"
      multiple
      class="hidden"
    />
  </div>
</template>
