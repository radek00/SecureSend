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
import { onMounted } from 'vue';
import { ref } from 'vue';
import { useField, useForm } from 'vee-validate'
import DateInput from '@/components/FileUploadForm/DateInput.vue';
import { computed } from 'vue';
import FileInput from '@/components/FileUploadForm/FileInput.vue';

const salt = crypto.getRandomValues(new Uint8Array(16));
const keychain = new AuthenticatedSecretKeyCryptography("password", salt);
const {value} = useField(() => 'files')
const step = ref<number>(0);

const stepZeroschema = {
    password(value: string) {
        if (value) return true;
        return false;
    }
};

const stepOneSchema = {
    date(value: string) {
        return true;
    }
}

const currentSchema = computed(() => {
    if (step.value === 0) return stepZeroschema;
    if (step.value === 1) return stepOneSchema;
})

const {handleSubmit, meta} = useForm({
    validationSchema: currentSchema
})


onMounted(async () => {
    await keychain.start();
})

const onSubmit = handleSubmit(values => {
    console.log(values, value)
    if (step.value === 2) {
        uploadFile();
    } else {
        step.value++;
    }
})

const uuid = self.crypto.randomUUID();
const uploadStatus = ref<number>();


const files =  ref(new Map<File, number>());

const onFilesChange = (event: any) => {
    files.value.clear();
    for (let i = 0; i < event.target.files.length; i++) {
        const file = event.target.files[i];

        files.value.set(file, 0);
    }
}

const uploadFile = async() => {
    try {
        await SecureSendService.createSecureUpload(uuid);
        await encryptFile();
    } catch (error) {
        console.log(error)
    }
}

const encryptFile = async () => {
    uploadStatus.value = 0;
    const requests: Promise<unknown>[] = [];
    for (const [file] of files.value) {
        const promise = splitFile(file, 64 * 1024, async (chunk: ArrayBuffer, num, totalChunks) => {
            await SecureSendService.uploadChunk(uuid, num, totalChunks, file.name, chunk);
            files.value.set(file, Math.ceil(((num + 1) / totalChunks) * 100))
        }, async (chunk, num) => await keychain.encrypt(chunk, num));
        requests.push(promise);
    }
    await Promise.all(requests);
}
</script>