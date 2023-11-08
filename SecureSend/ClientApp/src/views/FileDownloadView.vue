<script setup lang="ts">
import StyledButton from "@/components/StyledButton.vue";
import endpoints from "@/config/endpoints";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { ButtonType } from "@/models/enums/ButtonType";
import { verifyHash } from "@/utils/pbkdfHash";
import { ref } from "vue";
import SimpleInput from "@/components/SimpleInput.vue";
import { computed } from "vue";
import FileCard from "@/components/FileCard.vue";
import type { IWorkerInit } from "@/models/WorkerInit";

const props = defineProps<{
  secureUpload: SecureUploadDto;
  salt: Uint8Array;
  masterKey: string | Uint8Array;
  isPasswordProtected: boolean;
}>();

const password = ref<string>("");

const setUpWorker = () => {
  navigator.serviceWorker.controller?.postMessage({
    request: "init",
    id: props.secureUpload.secureUploadId,
    salt: props.salt,
    masterKey: props.isPasswordProtected ? password.value : props.masterKey,
  } as IWorkerInit);
};

if (!props.isPasswordProtected) setUpWorker();

const download = (url: string) => {
  const anchor = document.createElement("a");
  anchor.href = url;
  document.body.appendChild(anchor);
  anchor.click();
};

const isPasswordValid = ref<boolean>();

const verifyPassword = async () => {
  isPasswordValid.value = await verifyHash(
    props.masterKey as string,
    password.value
  );
  if (isPasswordValid.value) setUpWorker();
};

const isPasswordValidComputed = computed(
  () => isPasswordValid.value === true || isPasswordValid.value === undefined
);
</script>
<template>
  <div class="w-11/12 md:w-6/12" v-if="!isPasswordProtected || isPasswordValid">
    <h1 class="text-4xl text-white text-center">Download files</h1>
    <div
      class="flex flex-col gap-5 mt-5 w-full justify-between p-6 border rounded-lg shadow bg-gray-700 border-gray-600"
    >
      <FileCard
        v-for="file in secureUpload.files"
        :key="(file.fileName as string)"
        :file-name="file.fileName!"
        :size="file.fileSize"
      >
        <template #cardBottom>
          <a
            class="font-medium text-blue-500 hover:underline"
            href="#"
            @click.prevent
            @click="
              download(
                `${endpoints.download}?id=${secureUpload.secureUploadId}&fileName=${file.fileName}`
              )
            "
            >Download</a
          >
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
      <div class="mt-4 space-y-4 lg:mt-5 md:space-y-5">
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
          ></SimpleInput>
        </div>
        <StyledButton @click="verifyPassword()" :category="ButtonType.primary"
          >Unlock</StyledButton
        >
      </div>
    </div>
  </div>
</template>
