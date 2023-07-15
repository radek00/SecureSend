<template>
  <div
    class="w-11/12 md:w-6/12 flex flex-col justify-between gap-4 h-11/12 p-6 border border-gray-300 rounded-lg shadow dark:bg-gray-800 dark:border-gray-800"
  >
    <FormStepper :step="step"></FormStepper>
    <div v-if="step === 0">
      <SchemaInput
        name="password"
        type="password"
        label="Encryption password"
      ></SchemaInput>
    </div>
    <div v-if="step === 1">
      <SchemaInput name="date" type="date" label="Expiry date"></SchemaInput>
    </div>
    <div v-if="step === 2">
      <FileInput
        :files="files"
        @on-fiels-change="onFilesChange($event)"
      ></FileInput>
    </div>
    <div
      class="flex gap-5 md:gap-0 flex-col md:flex-row justify-between items-center"
    >
      <StyledButton
        :type="ButtonType.primary"
        :disabled="step === 0 || isLoading"
        @click="step -= 1"
        >Back</StyledButton
      >
      <StyledButton
        :type="ButtonType.primary"
        :disabled="!meta.valid || isLoading"
        @click="onSubmit()"
      >
        <span class="flex items-center justify-center">
          {{ step < 2 ? "Next" : "Upload" }}
          <LoadingIndicator
            v-if="isLoading && step !== 2"
            class="w-5 h-5 ml-2"
          ></LoadingIndicator>
        </span>
      </StyledButton>
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
      <StyledButton @click="copyToClipboard()" :type="ButtonType.primary"
        >Copy to clipboard</StyledButton
      >
    </template>
  </ConfirmModalVue>
</template>

<script setup lang="ts">
import SchemaInput from "@/components/FileUploadForm/SchemaInput.vue";
import { SecureSendService } from "@/services/SecureSendService";
import AuthenticatedSecretKeyCryptography from "@/utils/AuthenticatedSecretKeyCryptography";
import splitFile from "@/utils/splitFile";
import { ref, type Ref } from "vue";
import { useForm } from "vee-validate";
import { computed } from "vue";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import FormStepper from "@/components/FileUploadForm/FormStepper.vue";
import StyledButton from "@/components/FileUploadForm/StyledButton.vue";
import { ButtonType } from "@/models/enums/ButtonType";
import ConfirmModalVue from "@/components/ConfirmModal.vue";
import { useConfirmDialog } from "@/utils/composables/useConfirmDialog";
import SimpleInput from "@/components/SimpleInput.vue";
import { inject } from "vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";

interface IMappedFormValues {
  expiryDate: string;
  password: string;
}

const { isRevealed, reveal, confirm } = useConfirmDialog();

const salt = crypto.getRandomValues(new Uint8Array(16));
console.log(salt);
let keychain: AuthenticatedSecretKeyCryptography;
const step = ref<number>(0);

let downloadUrl: string;

const isLoading = inject<Ref<boolean>>("isLoading");

const stepZeroschema = {
  password(value: string) {
    if (value) return true;
    return "Password is required.";
  },
};

const stepOneSchema = {
  date(value: string) {
    if (new Date(value) <= new Date())
      return "Expiry date must be earlier than today.";
    return true;
  },
};

const currentSchema = computed(() => {
  if (step.value === 0) return stepZeroschema;
  if (step.value === 1) return stepOneSchema;
});

const getInitialValues = (): IMappedFormValues => {
  return {
    password: "",
    expiryDate: "",
  };
};

const { handleSubmit, meta, resetForm } = useForm({
  validationSchema: currentSchema,
  initialValues: getInitialValues(),
  keepValuesOnUnmount: true,
});

const onSubmit = handleSubmit(async (values: IMappedFormValues) => {
  if (step.value === 1) {
    isLoading!.value = true;
    await SecureSendService.createSecureUpload(uuid);
    isLoading!.value = false;
  } else if (step.value === 2) {
    isLoading!.value = true;
    keychain = new AuthenticatedSecretKeyCryptography(values.password, salt);
    await keychain.start();
    await encryptFile();
    isLoading!.value = false;
    const { data } = await reveal();
    console.log(data);
    if (data) {
      resetForm({ values: getInitialValues() });
      step.value = 0;
      return;
    }
    console.log("dialog closed");
  }
  if (step.value < 2) step.value++;
});

const copyToClipboard = () => navigator.clipboard.writeText(downloadUrl);

const uuid = self.crypto.randomUUID();
const uploadStatus = ref<number>();

const files = ref(new Map<File, number | string | boolean>());

const onFilesChange = (event: any) => {
  files.value.clear();
  for (let i = 0; i < event.target.files.length; i++) {
    const file = event.target.files[i];

    files.value.set(file, 0);
  }
};

const createDownloadUrl = () => {
  const base64Salt = btoa(String.fromCharCode.apply(null, salt as any));
  downloadUrl = window.location
    .toString()
    .concat(`download/${uuid}#${base64Salt}_${keychain.hash}`);
  return downloadUrl;
};

const encryptFile = async () => {
  uploadStatus.value = 0;
  const requests: Promise<unknown>[] = [];
  for (const [file] of files.value) {
    const promise = splitFile(
      file,
      5 * 1024 * 1024,
      async (chunk: ArrayBuffer, num, totalChunks) => {
        try {
          await SecureSendService.uploadChunk(
            uuid,
            num,
            totalChunks,
            file.name,
            chunk,
            file.type
          );
          files.value.set(
            file,
            num + 1 === totalChunks
              ? true
              : Math.ceil(((num + 1) / totalChunks) * 100)
          );
        } catch (error) {
          files.value.set(file, "Error with uploading file");
          throw error;
        }
      },
      async (chunk, num) => await keychain.encrypt(chunk, num)
    );
    requests.push(promise);
  }
  await Promise.all(requests);
};
</script>
