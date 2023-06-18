<template>
<div v-if="step === 0">
    <PasswordInput name="password"></PasswordInput>
</div>
<div v-if="step === 1">
    <DateInput name="date"></DateInput>
</div>
<div v-if="step === 2">
    <FileInput :files="files" @on-fiels-change="onFilesChange($event)"></FileInput>
</div>



<button :disabled="!meta.valid" @click="onSubmit()">{{ step < 2 ? 'Next' : 'Upload' }}</button>
</template>

<script setup lang="ts">
import PasswordInput from '@/components/FileUploadForm/PasswordInput.vue';
import { SecureSendService } from '@/services/SecureSendService';
import AuthenticatedSecretKeyCryptography from '@/utils/AuthenticatedSecretKeyCryptography';
import splitFile from '@/utils/splitFile';
import { ref } from 'vue';
import { useForm } from 'vee-validate'
import DateInput from '@/components/FileUploadForm/DateInput.vue';
import { computed } from 'vue';
import FileInput from '@/components/FileUploadForm/FileInput.vue';

interface IMappedFormValues {
    expiryDate: string;
    password: string
}

const salt = crypto.getRandomValues(new Uint8Array(16));
let keychain: AuthenticatedSecretKeyCryptography;
const step = ref<number>(0);

const stepZeroschema = {
    password(value: string) {
        if (value) return true;
        return 'Password is required.';
    }
};

const stepOneSchema = {
    date(value: string) {
        if (new Date(value) <= new Date()) return 'Expiry date must be earlier than today.';
        return true;
    }
}

const currentSchema = computed(() => {
    if (step.value === 0) return stepZeroschema;
    if (step.value === 1) return stepOneSchema;
})


const getInitialValues = (): IMappedFormValues => {
    return {
        password: '',
        expiryDate: ''
    }
}

const {handleSubmit, meta} = useForm({
    validationSchema: currentSchema,
    initialValues: getInitialValues(),
    keepValuesOnUnmount: true
})

const onSubmit = handleSubmit(async (values: IMappedFormValues) => {
    if (step.value === 1) {
        await SecureSendService.createSecureUpload(uuid);    
    }
    else if (step.value === 2) {
        keychain = new AuthenticatedSecretKeyCryptography(values.password, salt);
        await keychain.start();
        await encryptFile();
        alert(`Heres your link to share files: ${createDownloadUrl()}`)
    } 
    if (step.value < 2) step.value++;
})

const uuid = self.crypto.randomUUID();
const uploadStatus = ref<number>();


const files =  ref(new Map<File, number | string>());

const onFilesChange = (event: any) => {
    files.value.clear();
    for (let i = 0; i < event.target.files.length; i++) {
        const file = event.target.files[i];

        files.value.set(file, 0);
    }
}

const createDownloadUrl = () => {
    const base64Salt = btoa(String.fromCharCode.apply(null, salt as any))
    return window.location.toString().concat(`download/${uuid}#${base64Salt}`);
}

const encryptFile = async () => {
    uploadStatus.value = 0;
    const requests: Promise<unknown>[] = [];
    for (const [file] of files.value) {
        const promise = splitFile(file, 64 * 1024, async (chunk: ArrayBuffer, num, totalChunks) => {
            try {
                await SecureSendService.uploadChunk(uuid, num, totalChunks, file.name, chunk);
                files.value.set(file, Math.ceil(((num + 1) / totalChunks) * 100))
            } catch (error) {
                files.value.set(file, 'Error with uploading file')
                throw error;
            }

        }, async (chunk, num) => await keychain.encrypt(chunk, num));
        requests.push(promise);
    }
    await Promise.all(requests);
}
</script>