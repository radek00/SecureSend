<script setup lang="ts">
import StyledButton from "@/components/StyledButton.vue";
import endpoints from "@/config/endpoints";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { ButtonType } from "@/models/enums/ButtonType";
import type { Ref } from "vue";
import { computed, inject, ref } from "vue";
import SimpleInput from "@/components/SimpleInput.vue";
import FileCard from "@/components/FileCard.vue";
import type { IWorkerInit } from "@/models/WorkerInit";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";
import { SecureSendService } from "@/services/SecureSendService";
import { InvalidPasswordError } from "@/models/errors/ResponseErrors";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import { DownloadState, type DownloadStateTuple } from "@/models/DownloadState";

const props = defineProps<{
  verifyUploadResponse: UploadVerifyResponseDTO;
  b64Key: string;
}>();

const isLoading = inject<Ref<boolean>>("isLoading");

const secureUpload = ref<SecureUploadDto | null>(null);

const password = ref<string>("");

const fileDownloadStatuses = ref<Map<string, DownloadStateTuple>>(
  new Map<string, DownloadStateTuple>()
);

const broadcast = new BroadcastChannel("progress-channel");

broadcast.onmessage = (event) => {
  if (event.data.request === "progress") {
    fileDownloadStatuses.value.set(event.data.fileName, [
      event.data.value,
      DownloadState.InProgress,
    ]);

    console.log("progress", fileDownloadStatuses.value);
  }
};

const setUpWorker = async () => {
  navigator.serviceWorker.controller?.postMessage({
    request: "init",
    id: secureUpload.value!.secureUploadId,
    b64key: props.b64Key,
    password: props.verifyUploadResponse.isProtected
      ? password.value
      : undefined,
  } as IWorkerInit);
};

const viewSecureUpload = async () => {
  secureUpload.value = await SecureSendService.viewSecureUpload({
    id: props.verifyUploadResponse.secureUploadId,
    password: password.value,
  });
  await setUpWorker();
  secureUpload.value.files!.forEach((file) =>
      fileDownloadStatuses.value.set(file.fileName!, ["0%", DownloadState.New])
  );
}

if (!props.verifyUploadResponse.isProtected) {
  await viewSecureUpload();
}

const isPasswordValid = ref<boolean>();

const verifyPassword = async () => {
  isLoading!.value = true;
  try {
    await viewSecureUpload()
    isPasswordValid.value = true;

  } catch (err: unknown) {
    if (err instanceof InvalidPasswordError) isPasswordValid.value = false;
    else throw err;
  }
  isLoading!.value = false;
};

const isPasswordValidComputed = computed(
  () => isPasswordValid.value === true || isPasswordValid.value === undefined
);

const downloadAll = async () => {
  const directoryHandle = await window.showDirectoryPicker();
  const files = secureUpload.value?.files!;
  const promises = [];
  for (const file of files) {
    const promise = async (): Promise<void> => {
      try {
        const writableStream = await (
          await directoryHandle.getFileHandle(file.fileName!, { create: true })
        ).createWritable();
        const response = await fetch(
          `${endpoints.download}?id=${
            secureUpload.value!.secureUploadId
          }&fileName=${file.fileName}`
        );
        await response.body!.pipeTo(writableStream);
        fileDownloadStatuses.value.set(file.fileName!, [
          "Download completed",
          DownloadState.Completed,
        ]);
        console.log(fileDownloadStatuses.value);
      } catch {
        fileDownloadStatuses.value.set(file.fileName!, [
          "Error with downloading file",
          DownloadState.Failed,
        ]);
      }
    };
    promises.push(promise());
  }
  await Promise.all(promises);
};
</script>
<template>
  <div class="flex justify-center items-center pt-14 md:pt-20">
    <div
      class="w-11/12 md:w-6/12"
      v-if="!verifyUploadResponse.isProtected || isPasswordValid"
    >
      <h1 class="text-4xl text-white text-center">Download files</h1>
      <div
        class="max-h-[70vh] overflow-y-auto flex flex-col gap-5 mt-5 w-full justify-between p-6 border rounded-lg shadow bg-gray-700 border-gray-600"
      >
        <div v-if="true">
          <StyledButton :category="ButtonType.primary" @click="downloadAll()"
            >Download all</StyledButton
          >
        </div>
        <FileCard
          v-for="file in secureUpload!.files"
          :key="(file.fileName as string)"
          :file-name="file.fileName!"
          :size="file.fileSize"
        >
          <template #cardBottom>
            <a
              class="font-medium text-blue-500 hover:underline"
              :href="`${endpoints.download}?id=${secureUpload!.secureUploadId}&fileName=${file.fileName}`"
              >Download</a
            >
          </template>
          <template #cardMiddle>
            {{ fileDownloadStatuses.get(file.fileName!) }}
          </template>
        </FileCard>
      </div>
    </div>

    <div
      v-else
      class="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-0 w-full"
    >
      <a
        href="#"
        class="flex items-center mb-6 text-2xl font-semibold text-white"
      >
        SecureSend
      </a>
      <div
        class="w-full p-6 rounded-lg shadow border md:mt-0 sm:max-w-md bg-gray-800 border-gray-700 sm:p-8"
      >
        <h2
          class="mb-1 text-xl font-bold leading-tight tracking-tight md:text-2xl text-white"
        >
          Unlock your files
        </h2>
        <form
          @submit.prevent="verifyPassword()"
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
              :isValid="isPasswordValidComputed"
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
