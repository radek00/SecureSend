<script setup lang="ts">
import UploadIcon from "@/assets/icons/UploadIcon.vue";
import FileCard from "../FileCard.vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import CheckIcon from "@/assets/icons/CheckIcon.vue";
import { ref } from "vue";
import { useDropZone } from "@/utils/composables/useDropZone";
import TrashIcon from "@/assets/icons/TrashIcon.vue";
import PlusIcon from "@/assets/icons/PlusIcon.vue";
import { UploadStatus } from "@/models/enums/UploadStatus";
import { inject } from "vue";
import CloseIcon from "@/assets/icons/CloseIcon.vue";
import PauseIcon from "@/assets/icons/PauseIcon.vue";
import PlayIcon from "@/assets/icons/PlayIcon.vue";
import OptionsDropdown from "@/components/OptionsDropdown.vue";

const emit = defineEmits<{
  onFielsChange: [files: File[] | null];
  onFileRemove: [file: File];
  onCancel: [file: File];
  onPause: [file: File];
  onResume: [file: File];
}>();

defineProps<{
  files: Map<File, number | string | boolean>;
  isUploadSetup: boolean;
}>();

const fileDropZone = ref<HTMLElement>();

const onDrop = (files: File[] | null) => {
  emit("onFielsChange", files);
};

const isLoading = inject<boolean>("isLoading");

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
      class="flex flex-col items-center justify-center w-full h-64 border-2 border-dashed rounded-lg cursor-pointer hover:bg-bray-800 bg-gray-700 border-gray-600 hover:border-gray-500 hover:bg-gray-600"
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
    class="flex flex-col gap-5 w-full justify-between h-[300px] overflow-y-auto p-6 border rounded-lg shadow bg-gray-700 border-gray-600"
  >
    <TransitionGroup name="list">
      <FileCard
        v-for="([key, value], idx) in files"
        :key="idx"
        :file-name="key.name"
        :size="key.size"
      >
        <template #cardMiddle>
          <button
            v-if="!isLoading && value !== true"
            @click="emit('onFileRemove', key)"
            type="button"
            class="hidden md:block m-0 border hover:enabled:bg-red-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-red-500 hover:enabled:text-white focus:ring-red-800 hover:bg-red-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
          >
            <TrashIcon class="w-5 h-3"></TrashIcon>
            <span class="sr-only">Remove file</span>
          </button>
          <div v-if="isLoading" class="hidden md:flex gap-1 justify-between">
            <button
              @click="emit('onCancel', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-red-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-red-500 hover:enabled:text-white focus:ring-red-800 hover:bg-red-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <CloseIcon class="w-5 h-3"></CloseIcon>
              <span class="sr-only">Cancel</span>
            </button>
            <button
              v-if="value !== UploadStatus.paused"
              @click="emit('onPause', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-orange-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-orange-500 hover:enabled:text-white focus:ring-orange-800 hover:bg-orange-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <PauseIcon class="w-5 h-3"></PauseIcon>
              <span class="sr-only">Pause</span>
            </button>
            <button
              v-if="value === UploadStatus.paused"
              @click="emit('onResume', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-green-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-green-500 hover:enabled:text-white focus:ring-green-800 hover:bg-green-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <PlayIcon class="w-5 h-3"></PlayIcon>
              <span class="sr-only">Resume</span>
            </button>
          </div>

          <div class="block md:hidden">
            <OptionsDropdown>
              <li
                class="px-4 py-2 hover:bg-gray-600 hover:text-white"
                v-if="!isLoading && value !== true"
              >
                <a href="#" @click="emit('onFileRemove', key)">Remove</a>
              </li>
              <li
                class="px-4 py-2 hover:bg-gray-600 hover:text-white"
                v-if="isUploadSetup"
              >
                <a href="#" @click="emit('onCancel', key)">Cancel</a>
              </li>
              <li
                class="px-4 py-2 hover:bg-gray-600 hover:text-white"
                v-if="isUploadSetup && value !== UploadStatus.paused"
              >
                <a href="#" @click="emit('onPause', key)">Pause</a>
              </li>
              <li
                class="px-4 py-2 hover:bg-gray-600 hover:text-white"
                v-if="isUploadSetup && value === UploadStatus.paused"
              >
                <a href="#" @click="emit('onResume', key)">Resume</a>
              </li>
            </OptionsDropdown>
          </div>

          <LoadingIndicator
            v-if="value === 100"
            class="w-8 h-5 mr-2"
          ></LoadingIndicator>
          <CheckIcon v-if="value === true"></CheckIcon>
        </template>
        <template #cardBottom>
          <div class="w-full rounded-full bg-gray-700 mt-2">
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
                    ? UploadStatus.finishing
                    : `${value}%`
                  : UploadStatus.completed
              }}
            </div>
          </div>
        </template>
      </FileCard>
    </TransitionGroup>
    <label
      for="add-more-files"
      type="button"
      class="w-[fit-content] text-blue-600 border border-blue-700 hover:bg-blue-700 hover:text-white focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-full text-sm p-2.5 text-center inline-flex items-center"
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

<style scoped>
.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
