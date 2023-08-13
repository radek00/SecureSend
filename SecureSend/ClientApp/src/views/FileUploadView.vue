<template>
  <div
    class="w-11/12 md:w-6/12 flex flex-col justify-between gap-4 h-11/12 p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
  >
    <FormStepper class="px-[10px]" :step="step"></FormStepper>
    <div
      class="flex overflow-hidden items-center h-[100px] transition-height duration-500"
      :class="{ 'h-[100px]': step !== 2, 'h-[300px]': step === 2 }"
    >
      <div
        class="w-full shrink-0 transition-transform duration-700 px-[10px]"
        :style="{ transform }"
      >
        <SchemaInput
          name="password"
          type="password"
          label="Encryption password"
        ></SchemaInput>
      </div>
      <div
        class="w-full shrink-0 transition-transform duration-700 px-[10px]"
        :style="{ transform }"
      >
        <SchemaInput
          name="expiryDate"
          type="date"
          label="Expiry date"
        ></SchemaInput>
      </div>
      <div
        class="w-full shrink-0 transition-transform duration-700 px-[10px]"
        :style="{ transform }"
      >
        <FileInput
          :files="files"
          :is-upload-setup="isUploadSetup"
          @on-fiels-change="(value) => onFilesChange(value)"
          @on-cancel="(value) => onCancel(value)"
          @on-file-remove="
            (value) => {
              files.delete(value);
              fileKeys.delete(value.name);
            }
          "
        ></FileInput>
      </div>
    </div>
    <div
      class="flex gap-5 md:gap-0 flex-col md:flex-row justify-between items-center px-[10px]"
    >
      <StyledButton
        :type="ButtonType.primary"
        :disabled="step === 0 || isLoading || isUploadSetup"
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
            v-if="isLoading"
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
import SchemaInput from "@/components/SchemaInput.vue";
import { SecureSendService } from "@/services/SecureSendService";
import AuthenticatedSecretKeyCryptography from "@/utils/AuthenticatedSecretKeyCryptography";
import splitFile from "@/utils/splitFile";
import { ref, type Ref } from "vue";
import { useForm } from "vee-validate";
import { computed } from "vue";
import FileInput from "@/components/FileUploadForm/FileInput.vue";
import FormStepper from "@/components/FileUploadForm/FormStepper.vue";
import StyledButton from "@/components/StyledButton.vue";
import { ButtonType } from "@/models/enums/ButtonType";
import ConfirmModalVue from "@/components/ConfirmModal.vue";
import { useConfirmDialog } from "@/utils/composables/useConfirmDialog";
import SimpleInput from "@/components/SimpleInput.vue";
import { inject } from "vue";
import LoadingIndicator from "@/components/LoadingIndicator.vue";
import { useAlert } from "@/utils/composables/useAlert";
import { UploadStatus } from "@/models/enums/UploadStatus";

interface IMappedFormValues {
  expiryDate: string;
  password: string;
}

const transform = computed(() => `translateX(-${step.value * 100}%)`);

const { isRevealed, reveal, confirm } = useConfirmDialog();

const { openSuccess, openDanger } = useAlert();

let salt = crypto.getRandomValues(new Uint8Array(16));
let keychain: AuthenticatedSecretKeyCryptography;

const step = ref<number>(0);

let uuid = self.crypto.randomUUID();

const files = ref(new Map<File, number | string | boolean>());
const fileKeys = new Map<string, boolean>();
const controllers = new Map<string, AbortController>();

let downloadUrl: string;

const isLoading = inject<Ref<boolean>>("isLoading");

const isUploadSetup = ref<boolean>(false);

const stepZeroschema = {
  password(value: string) {
    if (value) return true;
    return "Password is required.";
  },
};

const stepOneSchema = {
  expiryDate(value: string) {
    if (new Date(value) <= new Date())
      return "Expiry date must be earlier than today.";
    return true;
  },
};

const currentSchema = computed(() => {
  if (step.value === 0) return stepZeroschema;
  return stepOneSchema;
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
  if (step.value === 2) {
    isLoading!.value = true;
    if (!isUploadSetup.value) {
      await SecureSendService.createSecureUpload(uuid, values.expiryDate);
      keychain = new AuthenticatedSecretKeyCryptography(values.password, salt);
      await keychain.start();
      isUploadSetup.value = true;
    }
    try {
      await encryptFile();
      isLoading!.value = false;
      if ([...files.value.values()].find((file) => file === true)) {
        const { data } = await reveal();
        if (data) {
          await formReset();
          openSuccess("Upload successful");
          return;
        }
      } else {
        openDanger("At least one file has to be uploaded to share files.");
        fileKeys.clear();
        files.value.clear();
        controllers.clear();
      }
    } catch (error) {
      openDanger("Upload failed, try again.");
      formReset();
    }
  }
  if (step.value < 2) step.value++;
});

const formReset = async () => {
  resetForm({ values: getInitialValues() });
  step.value = 0;
  salt = crypto.getRandomValues(new Uint8Array(16));
  uuid = self.crypto.randomUUID();
  files.value.clear();
  fileKeys.clear();
  controllers.clear();
  isUploadSetup.value = false;
  isLoading!.value = false;
};

const copyToClipboard = () => {
  navigator.clipboard.writeText(downloadUrl);
  openSuccess("Link copied to clipboard");
};

const onFilesChange = (formFiles: File[] | null) => {
  if (formFiles) {
    for (let i = 0; i < formFiles.length; i++) {
      const file = formFiles[i];
      if (!fileKeys.has(file.name)) {
        files.value.set(file, 0);
        fileKeys.set(file.name, true);
      }
    }
  }
};

const createDownloadUrl = () => {
  const base64Salt = btoa(String.fromCharCode(...salt));
  downloadUrl = window.location
    .toString()
    .concat(`download/${uuid}#${base64Salt}_${keychain.hash}`);
  return downloadUrl;
};

const encryptFile = async () => {
  const requests: Promise<unknown>[] = [];
  for (const [file] of files.value) {
    const promise = splitFile(
      file,
      5 * 1024 * 1024,
      async (chunk: ArrayBuffer, num, totalChunks) => {
        try {
          if (!controllers.has(file.name)) {
            const controller = new AbortController();
            controllers.set(file.name, controller);
          }
          await SecureSendService.uploadChunk(
            uuid,
            num,
            totalChunks,
            file.name,
            chunk,
            file.type,
            controllers.get(file.name)?.signal
          );
          files.value.set(
            file,
            num + 1 === totalChunks
              ? true
              : Math.ceil(((num + 1) / totalChunks) * 100)
          );
        } catch (error: any) {
          if (error.code === DOMException.ABORT_ERR) {
            files.value.set(file, UploadStatus.cancelled);
          } else {
            files.value.set(file, UploadStatus.error);
          }
          throw error;
        }
      },
      async (chunk, num) => await keychain.encrypt(chunk, num)
    );
    requests.push(promise);
  }
  const results = await Promise.allSettled(requests);
  if (
    results.find(
      (promise) =>
        promise.status === "rejected" &&
        !(promise.reason instanceof DOMException)
    )
  ) {
    throw new Error("Upload error");
  }
};

const onCancel = async (name: string) => {
  const controller = controllers.get(name);
  controller?.abort();
  await SecureSendService.cancelUpload({ id: uuid, fileName: name });
};
</script>
