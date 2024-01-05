<script setup lang="ts">
import UploadIcon from "@/assets/icons/UploadIcon.vue";
import FileCard from "../FileCard.vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import CheckIcon from "@/assets/icons/CheckIcon.vue";
import { inject, type Ref, ref } from "vue";
import { useDropZone } from "@/utils/composables/useDropZone";
import TrashIcon from "@/assets/icons/TrashIcon.vue";
import PlusIcon from "@/assets/icons/PlusIcon.vue";
import CloseIcon from "@/assets/icons/CloseIcon.vue";
import PauseIcon from "@/assets/icons/PauseIcon.vue";
import PlayIcon from "@/assets/icons/PlayIcon.vue";
import OptionsDropdown from "@/components/OptionsDropdown.vue";
import { UploadState, type UploadStateTuple } from "@/models/UploadStateTuple";

const emit = defineEmits<{
  onFilesChange: [files: FileList | undefined | null];
  onFileRemove: [file: File];
  onCancel: [file: File];
  onPause: [file: File];
  onResume: [file: File];
}>();

defineProps<{
  files: Map<File, UploadStateTuple>;
  isUploadSetup: boolean;
  step: number;
}>();

const isLoading = inject<Ref<boolean>>("isLoading");

const fileDropZone = ref<HTMLElement>();
const fileInput = ref<HTMLElement>();
const addMoreFilesInput = ref<HTMLElement>();

const onDrop = (files: FileList | undefined) => {
  emit("onFilesChange", files);
};

const { isOverDropZone } = useDropZone(fileDropZone, { onDrop });

const areOptionsAvailable = (state: UploadState) => {
  return (
    state === UploadState.NewFile ||
    state === UploadState.InProgress ||
    state === UploadState.Paused
  );
};
</script>

<template>
  <div
    v-if="!files.size"
    class="flex items-center justify-center w-full"
    ref="fileDropZone"
  >
    <label
      for="dropzone-file"
      :tabindex="step !== 2 ? -1 : 0"
      @keyup.enter="() => fileInput?.click()"
      class="flex flex-col items-center justify-center w-full h-64 border-2 border-dashed rounded-lg cursor-pointer bg-gray-700 border-gray-600 focus:border-gray-500 focus:bg-gray-600 hover:border-gray-500 hover:bg-gray-600"
    >
      <span class="flex flex-col items-center justify-center pt-5 pb-6">
        <UploadIcon></UploadIcon>
        <span v-if="!isOverDropZone" class="mb-2 text-sm text-gray-400">
          <span class="font-semibold">Click to upload</span> or drag and drop
        </span>
        <span v-else class="mb-2 text-sm text-gray-400">
          <span class="font-semibold">Drop files to upload</span>
        </span>
      </span>
      <input
        @change="
          $emit('onFilesChange', ($event.target as HTMLInputElement).files)
        "
        id="dropzone-file"
        type="file"
        class="opacity-0 absolute"
        multiple
        tabindex="-1"
        ref="fileInput"
      />
    </label>
  </div>
  <div
    v-else
    class="flex flex-col gap-5 w-full justify-between h-[300px] overflow-y-auto p-6 border rounded-lg shadow bg-gray-700 border-gray-600"
  >
    <TransitionGroup name="list">
      <FileCard
        v-for="[key, value] in files"
        :key="key.name"
        :file-name="key.name"
        :size="key.size"
      >
        <template #cardMiddle>
          <button
            v-if="value[1] === UploadState.NewFile && !isLoading"
            @click="emit('onFileRemove', key)"
            type="button"
            class="hidden md:inline-flex m-0 border hover:enabled:bg-red-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center items-center mr-2 border-red-500 hover:enabled:text-white focus:ring-red-800 hover:bg-red-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
          >
            <TrashIcon class="w-5 h-3"></TrashIcon>
            <span class="sr-only">Remove file</span>
          </button>
          <div class="hidden md:flex gap-1 justify-between">
            <button
              data-test="cancel-button"
              v-if="value[1] === UploadState.InProgress"
              @click="emit('onCancel', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-red-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-red-500 hover:enabled:text-white focus:ring-red-800 hover:bg-red-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <CloseIcon class="w-5 h-3"></CloseIcon>
              <span class="sr-only">Cancel</span>
            </button>
            <button
              data-test="pause-button"
              v-if="value[1] === UploadState.InProgress"
              @click="emit('onPause', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-orange-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-orange-500 hover:enabled:text-white focus:ring-orange-800 hover:bg-orange-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <PauseIcon class="w-5 h-3"></PauseIcon>
              <span class="sr-only">Pause</span>
            </button>
            <button
              data-test="resume-button"
              v-if="value[1] === UploadState.Paused"
              @click="emit('onResume', key)"
              type="button"
              :disabled="!isUploadSetup"
              class="m-0 border hover:enabled:bg-green-700 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm p-2.5 text-center inline-flex items-center mr-2 border-green-500 hover:enabled:text-white focus:ring-green-800 hover:bg-green-500 disabled:cursor-not-allowed disabled:bg-gray-600 disabled:border-gray-800"
            >
              <PlayIcon class="w-5 h-3"></PlayIcon>
              <span class="sr-only">Resume</span>
            </button>
          </div>

          <OptionsDropdown
            v-if="areOptionsAvailable(value[1])"
            class="block md:hidden"
          >
            <li
              v-if="value[1] === UploadState.NewFile && !isLoading"
              class="px-4 py-2 hover:bg-gray-600 hover:text-white"
            >
              <a href="#" @click="emit('onFileRemove', key)">Remove</a>
            </li>
            <li
              class="px-4 py-2 hover:bg-gray-600 hover:text-white"
              v-if="isUploadSetup && value[1] === UploadState.InProgress"
            >
              <a href="#" @click="emit('onCancel', key)">Cancel</a>
            </li>
            <li
              class="px-4 py-2 hover:bg-gray-600 hover:text-white"
              v-if="isUploadSetup && value[1] === UploadState.InProgress"
            >
              <a href="#" @click="emit('onPause', key)">Pause</a>
            </li>
            <li
              class="px-4 py-2 hover:bg-gray-600 hover:text-white"
              v-if="isUploadSetup && value[1] === UploadState.Paused"
            >
              <a href="#" @click="emit('onResume', key)">Resume</a>
            </li>
          </OptionsDropdown>

          <LoadingIndicator
            v-if="value[1] === UploadState.Merging"
            class="w-8 h-5 mr-2"
          ></LoadingIndicator>
          <CheckIcon v-if="value[1] === UploadState.Completed"></CheckIcon>
        </template>
        <template #cardBottom>
          <div class="w-full rounded-full bg-gray-700 mt-2">
            <div
              data-test="progress-bar"
              class="text-xs font-medium text-blue-100 text-center p-0.5 leading-none rounded-full"
              :class="{
                'bg-blue-600':
                  value[1] === UploadState.InProgress ||
                  value[1] === UploadState.NewFile,
                'bg-orange-600': value[1] === UploadState.Paused,
                'bg-green-600': value[1] === UploadState.Completed,
                'bg-red-600':
                  value[1] === UploadState.Failed ||
                  value[1] === UploadState.Cancelled,
              }"
              :style="{
                width: `${
                  value[1] === UploadState.InProgress ||
                  value[1] === UploadState.NewFile
                    ? value[0]
                    : `100%`
                }`,
              }"
            >
              {{ value[0] }}
            </div>
          </div>
        </template>
      </FileCard>
    </TransitionGroup>
    <div v-if="files.size > 0 && !isLoading">
      <label
        for="add-more-files"
        type="button"
        :tabindex="step !== 2 ? -1 : 0"
        @keyup.enter="() => addMoreFilesInput?.click()"
        class="w-[fit-content] text-blue-600 border border-blue-700 hover:bg-blue-700 hover:text-white focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-full text-sm p-2.5 text-center inline-flex items-center"
      >
        <PlusIcon class="w-4 h-4"></PlusIcon>
        <span class="ml-2">Add more files</span>
      </label>
      <input
        @change="
          $emit('onFilesChange', ($event.target as HTMLInputElement).files)
        "
        data-test="add-more-files"
        id="add-more-files"
        type="file"
        multiple
        class="opacity-0"
        ref="addMoreFilesInput"
        tabindex="-1"
      />
    </div>
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
