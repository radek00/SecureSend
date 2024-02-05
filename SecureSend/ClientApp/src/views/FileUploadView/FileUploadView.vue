<template>
  <div class="flex justify-center items-center pt-10 md:pt-20">
    <div
      class="w-11/12 lg:w-6/12 h-11/12 p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
    >
      <form @submit="onSubmit" class="flex flex-col justify-between gap-4">
        <FormStepper class="px-[10px]" :step="step"></FormStepper>
        <div class="flex overflow-hidden items-center h-auto">
          <div
            class="w-full shrink-0 transition-transform duration-700 px-[10px]"
            :style="{ transform }"
          >
            <SchemaInput
              name="password"
              type="password"
              label="Encryption password"
              :disabled="!values.isPasswordRequired"
            ></SchemaInput>
            <CheckboxSchemaInput
              name="isPasswordRequired"
              :checked-value="true"
              label="Password required"
              :tabindex="step !== 0 ? -1 : 0"
              autofocus
            ></CheckboxSchemaInput>
          </div>
          <div
            class="w-full shrink-0 transition-transform duration-700 px-[10px]"
            :style="{ transform }"
          >
            <SchemaInput
              :tabindex="step !== 1 ? -1 : 0"
              name="expiryDate"
              type="date"
              label="Optional expiry date"
            ></SchemaInput>
          </div>
          <div
            class="w-full shrink-0 transition-transform duration-700 px-[10px]"
            :style="{ transform }"
          >
            <FileInput
              v-show="step === 2"
              :step="step"
              :files="files"
              :is-upload-setup="isUploadSetup"
              @on-files-change="(value) => onFilesChange(value)"
              @on-cancel="(value) => onCancel(value)"
              @on-pause="(value) => onPause(value)"
              @on-resume="(value) => onResume(value)"
              @on-file-remove="(value) => onFileRemove(value)"
            ></FileInput>
          </div>
        </div>
        <div
          class="flex gap-3 md:gap-0 flex-col-reverse md:flex-row justify-between items-center px-[10px]"
        >
          <div
            class="flex gap-3 flex-col md:flex-row w-full md:w-auto md:gap-2"
          >
            <StyledButton
              type="button"
              class="w-full md:w-auto"
              :category="ButtonType.primary"
              :disabled="step === 0 || isLoading || isUploadSetup"
              @click="step -= 1"
              >Back</StyledButton
            >
            <StyledButton
              type="button"
              :disabled="(step === 0 && !meta.dirty) || isLoading"
              :category="ButtonType.cancel"
              class="w-full md:w-auto"
              @click="formReset()"
              >Reset</StyledButton
            >
          </div>
          <StyledButton
            class="w-full md:w-auto"
            :category="ButtonType.primary"
            :disabled="!meta.valid || isLoading || (step === 2 && !files.size) || isLimitExceeded"
            type="submit"
          >
            <span class="flex items-center justify-center">
              {{ step < 2 ? "Next" : "Upload" }}
              <LoadingIndicator
                v-if="isLoading"
                class="w-5 h-5 ml-2"
              ></LoadingIndicator>
            </span>
          </StyledButton>
        </div>
      </form>
      <ConfirmModalVue v-if="isRevealed" @close-click="confirm(true)">
        <template #header>Share your files</template>
        <template #body>
          <SimpleInput
            :value="createDownloadUrl()"
            label="Share your files"
            name="downloadUrl"
          ></SimpleInput>
        </template>
        <template #footer>
          <StyledButton
            @click="copyToClipboard()"
            :category="ButtonType.primary"
            >Copy to clipboard</StyledButton
          >
        </template>
      </ConfirmModalVue>
    </div>
  </div>
</template>

<script setup lang="ts">
import SchemaInput from "@/components/SchemaInput.vue";
import {computed, inject, provide, type Ref} from "vue";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import FormStepper from "@/components/FileUploadForm/FormStepper.vue";
import StyledButton from "@/components/StyledButton.vue";
import { ButtonType } from "@/models/enums/ButtonType";
import ConfirmModalVue from "@/components/ConfirmModal.vue";
import { useConfirmDialog } from "@/utils/composables/useConfirmDialog";
import SimpleInput from "@/components/SimpleInput.vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import { useAlert } from "@/utils/composables/useAlert";
import CheckboxSchemaInput from "@/components/CheckboxSchemaInput.vue";
import { useFileUploadForm } from "@/views/FileUploadView/useFileUploadForm";
import { UploadResult, useUpload } from "@/views/FileUploadView/useUpload";
import {useFileLimits} from "@/utils/composables/useFileLimits";

const transform = computed(() => `translateX(-${step.value * 100}%)`);

const { isRevealed, reveal, confirm } = useConfirmDialog();

const { openSuccess, openDanger } = useAlert();

const { handleSubmit, meta, values, resetUploadForm, step } =
  useFileUploadForm();
const {
  resetUpload,
  handleUpload,
  onResume,
  onPause,
  onFileRemove,
  onFilesChange,
  onCancel,
  createDownloadUrl,
  isUploadSetup,
  files,
} = useUpload(values);

const { isLimitExceeded, sizeLimit, totalSize } = useFileLimits(files);
provide("sizeLimits", { sizeLimit, totalSize, isLimitExceeded });

let downloadUrl: string;

const isLoading = inject<Ref<boolean>>("isLoading");
//handlers

//events

const showUploadResult = async (message: string) => {
  openSuccess(message);
  const { data } = await reveal();
  if (data) {
    return;
  }
};

const onSubmit = handleSubmit(async () => {
  if (step.value === 2) {
    const result = await handleUpload();
    switch (result) {
      case UploadResult.Success:
        await showUploadResult("Upload successful");
        formReset();
        break;

      case UploadResult.Partial:
        await showUploadResult("Only some files were uploaded");
        formReset();
        break;
      case UploadResult.AllCanceled:
        openDanger("At least one file has to be uploaded");
        break;
      case UploadResult.Failed:
        openDanger("Upload failed, try again");
        formReset();
        break;
      default:
        break;
    }
  } else {
    step.value++;
  }
});

//utils

const copyToClipboard = () => {
  navigator.clipboard.writeText(downloadUrl);
  openSuccess("Link copied to clipboard");
};

const formReset = () => {
  resetUploadForm();
  resetUpload();
};
</script>
