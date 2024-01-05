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
            :disabled="!meta.valid || isLoading || (step === 2 && !files.size)"
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
import { SecureSendService } from "@/services/SecureSendService";
import AuthenticatedSecretKeyCryptographyService from "@/utils/AuthenticatedSecretKeyCryptographyService";
import splitFile from "@/utils/splitFile";
import { computed, inject, ref, type Ref } from "vue";
import { useForm } from "vee-validate";
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
import { UploadState, type UploadStateTuple } from "@/models/UploadStateTuple";

interface IMappedFormValues {
  expiryDate: string;
  password: string;
  isPasswordRequired: boolean;
}

const transform = computed(() => `translateX(-${step.value * 100}%)`);

const { isRevealed, reveal, confirm } = useConfirmDialog();

const { openSuccess, openDanger } = useAlert();

let keychain: AuthenticatedSecretKeyCryptographyService;

const step = ref<number>(0);

let uuid = self.crypto.randomUUID();

const files = ref(new Map<File, UploadStateTuple>());
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

//handlers
const handleUploadCallback = async (
  chunk: ArrayBuffer,
  num: number,
  totalChunks: number,
  file: File
) => {
  try {
    await handlePause(file);
    const currentChunk = num + 1;
    await SecureSendService.uploadChunk(
      uuid,
      currentChunk,
      totalChunks,
      file.name,
      chunk,
      file.type,
      file.size,
      controllers.get(file.name)?.signal
    );
    const progress = Math.ceil((currentChunk / totalChunks) * 100);
    const stateTuple: UploadStateTuple =
      currentChunk === totalChunks
        ? ["Upload completed", UploadState.Completed]
        : currentChunk === totalChunks - 1
        ? ["Finishing upload...", UploadState.Merging]
        : [`${progress}%`, UploadState.InProgress];
    files.value.set(file, stateTuple);
  } catch (error: any) {
    if (
      error === UploadState.Cancelled ||
      error.code === DOMException.ABORT_ERR
    ) {
      files.value.set(file, ["Upload cancelled.", UploadState.Cancelled]);
    } else {
      files.value.set(file, ["Error with uploading file.", UploadState.Failed]);
    }
    throw error;
  }
};

const handleUploadResult = async () => {
  if (
    [...files.value.values()].find((file) => file[1] === UploadState.Completed)
  ) {
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
};

const handlePause = async (file: File) => {
  if (!controllers.has(file.name)) {
    const controller = new AbortController();
    controller.signal.addEventListener("pause", pauseEventListener);
    controllers.set(file.name, controller);
  }
  if (pausedFiles.get(file)) {
    files.value.set(file, ["Upload paused", UploadState.Paused]);
    const promise = new Promise((res) => {
      pausedFiles.set(file, res);
    });
    await promise;
  }
};

//events

const onSubmit = handleSubmit(async () => {
  if (step.value === 2) {
    isLoading!.value = true;
    if (!isUploadSetup.value) {
      await setupUpload();
      isUploadSetup.value = true;
    }
    try {
      await encryptFile();
      isLoading!.value = false;
      await handleUploadResult();
    } catch (error) {
      if (
        ![...files.value.values()].find(
          (file) => file[1] === UploadState.Completed
        )
      ) {
        openDanger("Upload failed, try again.");
        formReset();
      } else {
        await handleUploadResult();
      }
    }
  } else {
    step.value++;
  }
});
const onCancel = async (fileObj: File) => {
  const controller = controllers.get(fileObj.name);
  if (controller) {
    controller.abort(UploadState.Cancelled);
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

const onFilesChange = (formFiles: FileList | undefined | null) => {
  if (formFiles) {
    for (let i = 0; i < formFiles.length; i++) {
      const file = formFiles[i];
      if (!fileKeys.has(file.name)) {
        files.value.set(file, [`0%`, UploadState.NewFile]);
        fileKeys.set(file.name, true);
      }
    }
  }
};

//utils

const encryptFile = async () => {
  const requests: Promise<unknown>[] = [];
  for (const [file] of files.value) {
    const promise = splitFile(
      file,
      5 * 1024 * 1024,
      async (chunk: ArrayBuffer, num, totalChunks) => {
        await handleUploadCallback(chunk, num, totalChunks, file);
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
        promise.reason.code !== DOMException.ABORT_ERR &&
        promise.reason !== UploadState.Cancelled
    )
  ) {
    throw new Error("Upload error");
  }
};
const createDownloadUrl = () => {
  downloadUrl = window.location.origin
    .toString()
    .concat(`/download/${uuid}#${keychain.getSecret()}`);
  return downloadUrl;
};

const setupUpload = async () => {
  await SecureSendService.createSecureUpload({
    uploadId: uuid,
    expiryDate: values.expiryDate ? values.expiryDate : null,
    password: values.password,
  });
  keychain = new AuthenticatedSecretKeyCryptographyService(
    values.password ? values.password : undefined
  );
  await keychain.start();
};

const copyToClipboard = () => {
  navigator.clipboard.writeText(downloadUrl);
  openSuccess("Link copied to clipboard");
};

const formReset = () => {
  resetForm({ values: getInitialValues() });
  step.value = 0;
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

//listeners
const pauseEventListener = (event: any) => {
  pausedFiles.set(event.detail.file, true);
};
</script>
