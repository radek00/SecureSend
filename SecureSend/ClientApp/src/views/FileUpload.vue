<template>
<div class="button-wrap">
    <input ref="fileInput" type="file" multiple>
    <button @click="uploadFile()">Upload</button>
    <button>Download</button>
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
    const files: File[] = fileInput.value.files;
    const requests: Promise<unknown>[] = [];
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const promise = splitFile(file, 64 * 1024, async (chunk: ArrayBuffer, num, totalChunks) => {
            await SecureSendService.uploadChunk(uuid, num, totalChunks, file.name, chunk);
        }, async (chunk, num) => await keychain.encrypt(chunk, num));
        requests.push(promise);
    }
    await Promise.all(requests);
}
</script>

<style scoped>
.button-wrap {
    display: flex;
    gap: 10px;
}
</style>