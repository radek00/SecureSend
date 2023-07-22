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

const props = defineProps<{
  secureUpload: SecureUploadDto;
  salt: Uint8Array;
  passwordHash: string;
}>();

interface IWorkerInit {
  request: string;
  salt: Uint8Array;
  password: string;
  id: string;
}

const password = ref<string>("");

const setUpWorker = () => {
  navigator.serviceWorker.controller?.postMessage({
    request: "init",
    id: props.secureUpload.secureUploadId,
    salt: props.salt,
    password: password.value,
  } as IWorkerInit);
};

const download = (url: string) => {
  const anchor = document.createElement("a");
  anchor.href = url;
  document.body.appendChild(anchor);
  anchor.click();
};

const isPasswordValid = ref<boolean>();

const verifyPassword = async () => {
  isPasswordValid.value = await verifyHash(props.passwordHash, password.value);
  if (isPasswordValid.value) setUpWorker();
};

const isPasswordValidComputed = computed(
  () => isPasswordValid.value === true || isPasswordValid.value === undefined
);
</script>
<template>
  <div class="w-11/12 md:w-6/12" v-if="isPasswordValid">
    <h1 class="text-4xl text-gray-900 dark:text-white text-center">
      Download files
    </h1>
    <div
      class="flex flex-col gap-5 mt-5 w-full justify-between p-6 border border-gray-300 rounded-lg shadow dark:bg-gray-700 dark:border-gray-600"
    >
      <FileCard
        v-for="file in secureUpload.files"
        
        :file-name="file.fileName!"
      >
        <template #cardBottom>
          <a
            class="font-medium text-blue-600 dark:text-blue-500 hover:underline"
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
    class="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0 w-full"
  >
    <a
      href="#"
      class="flex items-center mb-6 text-2xl font-semibold text-gray-900 dark:text-white"
    >
      SecureSend
    </a>
    <div
      class="w-full p-6 bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md dark:bg-gray-800 dark:border-gray-700 sm:p-8"
    >
      <h2
        class="mb-1 text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white"
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
        <StyledButton @click="verifyPassword()" :type="ButtonType.primary"
          >Unlock</StyledButton
        >
      </div>
    </div>
  </div>
</template>
