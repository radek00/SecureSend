<template>
<div class="button-wrap">
    <input ref="fileInput" type="file" multiple @change="onFilesChange($event)">
    <button @click="uploadFile()">Upload</button>
</div>

<div class="button-wrap">
    <div v-for="[key, value] in files">
        {{ key.name }} = {{ value }}
    </div>
</div>
</template>

<script setup lang="ts">
import { SecureSendService } from '@/services/SecureSendService';
import AuthenticatedSecretKeyCryptography from '@/utils/AuthenticatedSecretKeyCryptography';
import splitFile from '@/utils/splitFile';
import { onMounted } from 'vue';
import { ref } from 'vue';

const salt = crypto.getRandomValues(new Uint8Array(16));
const keychain = new AuthenticatedSecretKeyCryptography("password", salt);

onMounted(async () => {
    await keychain.start();
})

const fileInput = ref();
const uuid = self.crypto.randomUUID();
const uploadStatus = ref<number>();

const files =  ref(new Map());

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

<style scoped>
.button-wrap {
    display: flex;
    flex-direction: column;
    gap: 10px;
}
</style>