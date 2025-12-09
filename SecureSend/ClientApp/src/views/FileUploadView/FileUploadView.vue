<template>
  <form
    @submit="onSubmit"
    class="flex flex-col lg:flex-row justify-center pt-10 md:pt-20 gap-5 items-start w-full max-w-7xl mx-auto px-4"
  >
    <!-- Left Column: Settings & History -->
    <div class="w-full lg:w-1/3 flex flex-col gap-5">
      <!-- Settings Panel -->
      <div class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800">
        <h2
          class="text-xl font-bold mb-4 text-white border-b border-gray-700 pb-2"
        >
          SETTINGS
        </h2>
        <div class="flex flex-col gap-8">
          <div>
            <SchemaInput
              name="password"
              type="password"
              label="Encryption password:"
              data-test="password"
              :disabled="!values.isPasswordRequired"
            ></SchemaInput>
            <CheckboxSchemaInput
              name="isPasswordRequired"
              :checked-value="true"
              label="Password required"
              autofocus
            ></CheckboxSchemaInput>
          </div>
          <SchemaInput
            name="expiryDate"
            type="date"
            :label="
              dateLimit === '' ? 'Optional expiration date:' : `Expire after`
            "
            data-test="expirationDate"
          >
            <template #label>
              <span
                class="float-right bg-yellow-100 text-yellow-800 text-xs font-medium px-2.5 py-0.5 rounded dark:bg-yellow-900 dark:text-yellow-300"
                >{{ `Max allowed: ${dateLimit}` }}</span
              >
            </template>
          </SchemaInput>
          <StyledButton
            type="button"
            :disabled="!meta.dirty || isLoading"
            :category="ButtonType.cancel"
            class="w-full"
            @click="formReset()"
            >Reset</StyledButton
          >
        </div>
      </div>

      <!-- Upload History Panel -->
      <div
        class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
        v-if="storageItem.length > 0"
      >
        <h2
          class="text-xl font-bold mb-4 text-white border-b border-gray-700 pb-2"
        >
          UPLOAD HISTORY
        </h2>
        <UploadHistory :uploads="storageItem"></UploadHistory>
      </div>
    </div>

    <!-- Right Column: File Upload & Queue -->
    <div class="w-full lg:w-2/3 flex flex-col gap-5">
      <div
        class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800 h-full flex flex-col justify-between"
      >
        <div>
          <h2
            class="text-xl font-bold mb-4 text-white border-b border-gray-700 pb-2"
          >
            FILE UPLOAD AREA
          </h2>
          <FileInput
            :step="2"
            :files="files"
            :is-upload-setup="isUploadSetup"
            @on-files-change="(value) => onFilesChange(value)"
            @on-cancel="(value) => onCancel(value)"
            @on-pause="(value) => onPause(value)"
            @on-resume="(value) => onResume(value)"
            @on-file-remove="(value) => onFileRemove(value)"
          ></FileInput>
        </div>

        <div class="mt-5">
          <StyledButton
            class="w-full py-4 text-lg"
            :category="ButtonType.primary"
            :disabled="
              !meta.valid || isLoading || !files.size || isLimitExceeded
            "
            type="submit"
          >
            <span class="flex items-center justify-center">
              Upload
              <LoadingIndicator
                v-if="isLoading"
                class="w-5 h-5 ml-2"
              ></LoadingIndicator>
            </span>
          </StyledButton>
        </div>
      </div>
    </div>

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
        <StyledButton @click="copyToClipboard()" :category="ButtonType.primary"
          >Copy to clipboard</StyledButton
        >
      </template>
    </ConfirmModalVue>
  </form>
</template>

<script setup lang="ts">
import SchemaInput from "@/components/SchemaInput.vue";
import { inject, provide, type Ref } from "vue";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
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
import { useFileLimits } from "@/utils/composables/useFileLimits";
import UploadHistory from "@/components/UploadHistory/UploadHistory.vue";
import type { SecureFileDto } from "@/models/SecureFileDto";
import type { HistoryItem } from "@/models/HistoryItem";
import { useLocalStorage } from "@/utils/composables/useLocalStorage";

const { isRevealed, reveal, confirm } = useConfirmDialog();

const { openSuccess, openDanger } = useAlert();

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
} = useUpload();

const { isLimitExceeded, sizeLimit, totalSize, dateLimit } =
  useFileLimits(files);
provide("sizeLimits", { sizeLimit, totalSize, isLimitExceeded });

const { handleSubmit, meta, values, resetUploadForm } =
  useFileUploadForm(dateLimit);

const { setItem, storageItem } = useLocalStorage<HistoryItem[]>("uploads", []);
storageItem.value = storageItem.value.filter(
  (item) => new Date(item.expirationDate) > new Date()
);

const isLoading = inject<Ref<boolean>>("isLoading");

const showUploadResult = async (message: string) => {
  openSuccess(message);
  const { data } = await reveal();
  if (data) {
    return;
  }
};

const onSubmit = handleSubmit(async (values) => {
  const result = await handleUpload(values);
  switch (result) {
    case UploadResult.Success:
      addToHistory();
      await showUploadResult("Upload successful");
      formReset();
      break;

    case UploadResult.Partial:
      addToHistory();
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
});

const addToHistory = () => {
  const uploadedFiles: Partial<SecureFileDto>[] = [];
  files.value.forEach((_value, file) => {
    uploadedFiles.push({ fileName: file.name, fileSize: file.size });
  });
  storageItem.value.push({
    link: createDownloadUrl(),
    uploadDate: new Date().toDateString(),
    files: uploadedFiles,
    expirationDate: values.expiryDate,
  });
  setItem();
};

const copyToClipboard = () => {
  navigator.clipboard.writeText(createDownloadUrl());
  openSuccess("Link copied to clipboard");
};

const formReset = () => {
  resetUploadForm();
  resetUpload();
};
</script>
