<template>
  <div
    class="w-11/12 md:w-6/12 flex flex-col justify-between gap-4 h-11/12 p-6 border rounded-lg shadow bg-gray-800 border-gray-800"
  >
    <FormStepper class="px-[10px]" :step="step"></FormStepper>
    <div
      class="flex overflow-hidden items-center h-[150px] transition-height duration-500"
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
          :disabled="!values.isPasswordRequired"
        ></SchemaInput>
        <CheckboxSchemaInput
          name="isPasswordRequired"
          :checked-value="true"
          label="Password required"
        ></CheckboxSchemaInput>
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
          @on-pause="(value) => onPause(value)"
          @on-resume="(value) => onResume(value)"
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
import CheckboxSchemaInput from "@/components/CheckboxSchemaInput.vue";

interface IMappedFormValues {
  expiryDate: string;
  password: string;
  isPasswordRequired: boolean;
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
const pausedFiles = new Map<File, boolean | ((value?: string) => void)>();
const controllers = new Map<string, AbortController>();

let downloadUrl: string;

const isLoading = inject<Ref<boolean>>("isLoading");

const isUploadSetup = ref<boolean>(false);

const stepZeroschema = {
  password(value: string) {
    if (!values.isPasswordRequired) return true;
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
    isPasswordRequired: false,
  };
};

const { handleSubmit, meta, resetForm, values } = useForm({
  validationSchema: currentSchema,
  initialValues: getInitialValues(),
  keepValuesOnUnmount: true,
});

const onSubmit = handleSubmit(async (values: IMappedFormValues) => {
  if (step.value === 2) {
    isLoading!.value = true;
    if (!isUploadSetup.value) {
      await SecureSendService.createSecureUpload(uuid, values.expiryDate);
      keychain = new AuthenticatedSecretKeyCryptography(salt, values.password ? values.password : undefined);
      await keychain.start();
      isUploadSetup.value = true;
    }
    try {
      await encryptFile();
      isLoading!.value = false;
      if ([...files.value.values()].find((file) => file === true)) {
        const { data } = await reveal();
        if (data) {
          formReset();
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

const formReset = () => {
  resetForm({ values: getInitialValues() });
  step.value = 0;
  salt = crypto.getRandomValues(new Uint8Array(16));
  uuid = self.crypto.randomUUID();
  files.value.clear();
  fileKeys.clear();
  [...controllers.values()].forEach((con) =>
    con.signal.removeEventListener("pause", pauseEventListener)
  );
  controllers.clear();
  isUploadSetup.value = false;
  isLoading!.value = false;
  pausedFiles.clear();
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
  const key = keychain.hash ?? btoa(String.fromCharCode(...keychain.getMasterKey()))
  downloadUrl = window.location
    .toString()
    .concat(`download/${uuid}?pass=${values.isPasswordRequired}#${base64Salt}_${key}`);
  return downloadUrl;
};

const pauseEventListener = (event: any) => {
  pausedFiles.set(event.detail.file, true);
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
            controller.signal.addEventListener("pause", pauseEventListener);
            controllers.set(file.name, controller);
          }
          if (pausedFiles.get(file)) {
            files.value.set(file, UploadStatus.paused);
            const promise = new Promise((res) => {
              pausedFiles.set(file, res);
            });
            await promise;
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
          if (error === UploadStatus.cancelled) {
            files.value.set(file, UploadStatus.cancelled);
          } else if (error === UploadStatus.paused) {
            files.value.set(file, UploadStatus.paused);
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
        promise.reason !== UploadStatus.cancelled
    )
  ) {
    throw new Error("Upload error");
  }
};

const onCancel = async (fileObj: File) => {
  const controller = controllers.get(fileObj.name);
  if (controller) {
    controller.abort(UploadStatus.cancelled);
    onResume(fileObj);
    await SecureSendService.cancelUpload({ id: uuid, fileName: fileObj.name });
  }
};

const onPause = (fileObj: File) => {
  const controller = controllers.get(fileObj.name);
  controller?.signal.dispatchEvent(
    new CustomEvent("pause", { detail: { file: fileObj } })
  );
};

const onResume = (fileObj: File) => {
  const resolve = pausedFiles.get(fileObj);
  if (resolve && typeof resolve == "function") {
    resolve();
    pausedFiles.set(fileObj, false);
  }
};
</script>
