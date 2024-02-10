<script setup lang="ts">
import StyledButton from "@/components/StyledButton.vue";
import endpoints from "@/config/endpoints";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { ButtonType } from "@/models/enums/ButtonType";
import type { Ref } from "vue";
import { inject, ref } from "vue";
import SimpleInput from "@/components/SimpleInput.vue";
import FileCard from "@/components/FileCard.vue";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import ProgressBar from "@/components/ProgressBar.vue";
import { useCheckHasFeature } from "@/utils/composables/useCheckHasFeature";
import { useDownloadAll } from "@/views/FileDownloadView/useDownload";
import { useDownloadForm } from "@/views/FileDownloadView/useDownloadForm";

const props = defineProps<{
  verifyUploadResponse: UploadVerifyResponseDTO;
  b64Key: string;
}>();

const isDownloadAllAvailable = useCheckHasFeature("showDirectoryPicker");
const { downloadAll, setupDownload, fileDownloadStatuses } = useDownloadAll();
const { isPasswordValid, verifyPassword, password } = useDownloadForm();

const isLoading = inject<Ref<boolean>>("isLoading");
const secureUpload = ref<SecureUploadDto | null | undefined>(null);

const viewSecureUpload = async () => {
  const upload = await verifyPassword(
    props.verifyUploadResponse.secureUploadId
  );
  console.log(upload, isPasswordValid.value);
  if (isPasswordValid.value) {
    secureUpload.value = upload;
    setupDownload(
      secureUpload.value!,
      props.b64Key,
      props.verifyUploadResponse.isProtected ? password.value : undefined
    );
  }
};

if (!props.verifyUploadResponse.isProtected) {
  await viewSecureUpload();
}
</script>
<template>
  <div class="flex justify-center items-center mt-14 md:mt-20">
    <div
      class="w-11/12 md:w-6/12"
      v-if="!verifyUploadResponse.isProtected || isPasswordValid"
    >
      <h1 class="text-4xl text-white text-center">Download files</h1>
      <div
        class="max-h-[70vh] overflow-y-auto flex flex-col gap-5 mt-5 w-full justify-between p-6 border rounded-lg shadow bg-gray-700 border-gray-600"
      >
        <div v-if="isDownloadAllAvailable">
          <StyledButton
            :category="ButtonType.primary"
            @click="downloadAll(secureUpload!)"
            >Download all</StyledButton
          >
        </div>
        <FileCard
          v-for="[fileName, status] in fileDownloadStatuses"
          :key="fileName"
          :file-name="fileName"
          :size="secureUpload!.files!.find((f) => f.fileName === fileName)?.fileSize"
        >
          <template #cardBottom>
            <a
              class="font-medium text-blue-500 hover:underline"
              :href="`${endpoints.download}?id=${secureUpload!.secureUploadId}&fileName=${fileName}`"
              >Download</a
            >
            <ProgressBar :state="status"></ProgressBar>
          </template>
        </FileCard>
      </div>
    </div>

    <div
      v-else
      class="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-0 w-full"
    >
      <div
        class="w-full p-6 rounded-lg shadow border md:mt-0 sm:max-w-md bg-gray-800 border-gray-700 sm:p-8"
      >
        <h2
          class="mb-1 text-xl font-bold leading-tight tracking-tight md:text-2xl text-white"
        >
          Unlock your files
        </h2>
        <form
          @submit.prevent="viewSecureUpload()"
          class="mt-4 space-y-4 lg:mt-5 md:space-y-5"
        >
          <div>
            <SimpleInput
              name="password"
              placeholder="••••••••"
              label="Decryption password"
              type="password"
              v-model="password"
              :value="password"
              :isValid="isPasswordValid"
              errorMessage="Invalid password"
              autofocus
            ></SimpleInput>
          </div>
          <StyledButton :category="ButtonType.primary" type="submit">
            <span class="flex items-center justify-center">
              Unlock
              <LoadingIndicator
                v-if="isLoading"
                class="w-5 h-5 ml-2"
              ></LoadingIndicator>
            </span>
          </StyledButton>
        </form>
      </div>
    </div>
  </div>
</template>
