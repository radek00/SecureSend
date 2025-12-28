<template>
  {{ values }}
  {{ meta }}
  <div class="w-full flex justify-center">
    <!-- Mobile View -->
    <section
      v-if="!isDesktop(screenType)"
      class="w-11/12 h-11/12 flex-col items-center mt-10 gap-5 flex"
    >
      <div
        class="w-full p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
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
                label="Encryption password:"
                data-test="password"
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
                :label="
                  dateLimit === ''
                    ? 'Optional expiration date:'
                    : `Expire after`
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
            class="flex gap-3 flex-col-reverse justify-between items-center px-[10px]"
          >
            <div class="flex gap-3 flex-col w-full">
              <StyledButton
                type="button"
                class="w-full"
                :category="ButtonType.primary"
                :disabled="step === 0 || isLoading || isUploadSetup"
                @click="step -= 1"
                >Back</StyledButton
              >
              <StyledButton
                type="button"
                :disabled="(step === 0 && !meta.dirty) || isLoading"
                :category="ButtonType.cancel"
                class="w-full"
                @click="formReset()"
                >Reset</StyledButton
              >
            </div>
            <StyledButton
              class="w-full"
              :category="ButtonType.primary"
              :disabled="
                !meta.valid ||
                isLoading ||
                (step === 2 && !files.size) ||
                isLimitExceeded
              "
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
      </div>
      <div
        class="w-full p-4 border rounded-lg shadow sm:p-8 bg-gray-800 border-gray-800"
        v-if="storageItem.length > 0"
      >
        <h2 class="text-xl font-bold text-white border-b border-gray-700 pb-2">
          Upload history
        </h2>
        <UploadHistory :uploads="storageItem"></UploadHistory>
      </div>
    </section>

    <!-- Desktop View -->
    <section
      v-else
      class="flex flex-row justify-center p-10 gap-5 items-start max-w-7xl w-screen mx-auto"
    >
      <!-- Left Column: Settings & History -->
      <div class="w-1/3 flex flex-col gap-5">
        <!-- Settings Panel -->
        <form
          @submit="onSubmit"
          class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
        >
          <h2
            class="text-xl font-bold mb-4 text-white border-b border-gray-700 pb-2"
          >
            Settings
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
        </form>

        <!-- Upload History Panel -->
        <div
          class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
          v-if="storageItem.length > 0"
        >
          <h2
            class="text-xl font-bold text-white border-b border-gray-700 pb-2"
          >
            Upload history
          </h2>
          <UploadHistory :uploads="storageItem"></UploadHistory>
        </div>
      </div>

      <!-- Right Column: File Upload & Queue -->
      <div class="w-2/3 flex flex-col gap-5">
        <div
          class="p-6 border rounded-lg shadow bg-gray-800 border-gray-800 h-full flex flex-col justify-between"
        >
          <div>
            <h2
              class="text-xl font-bold mb-4 text-white border-b border-gray-700 pb-2"
            >
              Files
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
              @click="onSubmit"
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
    </section>

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
          type="button"
          @click="copyToClipboard()"
          :category="ButtonType.primary"
          >Copy to clipboard</StyledButton
        >
      </template>
    </ConfirmModalVue>
  </div>
</template>

<script setup lang="ts">
//todo:
//remove setting step to 2/use isDesktop flag
//add desktop tests
import SchemaInput from "@/components/SchemaInput.vue";
import { computed, inject, provide, type Ref } from "vue";
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
import { useFileLimits } from "@/utils/composables/useFileLimits";
import UploadHistory from "@/components/UploadHistory/UploadHistory.vue";
import type { SecureFileDto } from "@/models/SecureFileDto";
import type { HistoryItem } from "@/models/HistoryItem";
import { useLocalStorage } from "@/utils/composables/useLocalStorage";
import { isDesktop, useScreenSize } from "@/utils/composables/useScreenSize";

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

const { screenType } = useScreenSize();

const { handleSubmit, meta, values, resetUploadForm, step } = useFileUploadForm(
  dateLimit,
  screenType
);

const transform = computed(() => `translateX(-${step.value * 100}%)`);

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

const handleUploadResult = async (result: UploadResult) => {
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
};

const onSubmit = handleSubmit(async (values) => {
  if (isDesktop(screenType.value)) {
    const result = await handleUpload(values);
    await handleUploadResult(result);
    return;
  }

  if (step.value === 2) {
    const result = await handleUpload(values);
    await handleUploadResult(result);
  } else {
    step.value++;
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
  resetUpload();
  resetUploadForm();
};
</script>
