<template>
<div class="button-wrap">
    <input ref="fileInput" type="file">
    <button @click="uploadFile()">Upload</button>
    <button>Download</button>
</div>
</template>

<script setup lang="ts">
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
    await setUpUpload();
    await encryptFile();
}

const setUpUpload = async () => {
    await fetch(`api/SecureSend?uploadId=${uuid}`, {
        "method": "POST",
    });
}

const uploadChunk = async(id: string, chunkNumber: number, totalChunks: number, name: string, chunk: ArrayBuffer) => {
    const formData = new FormData();
    formData.append('chunk', new Blob([chunk]), name)
    uploadStatus.value = Math.ceil(((chunkNumber + 1) / totalChunks) * 100);
    chunkNumber = +chunkNumber + 1;
    const requestOptions = {
        method: 'POST',
        body: formData
    }
    const response = await fetch(`api/SecureSend/uploadChunks?uploadId=${id}&chunkNumber=${chunkNumber}&totalChunks=${totalChunks}`, requestOptions);
    if (!response.ok) throw response.statusText;

}

const encryptFile = async () => {
  uploadStatus.value = 0;
  const file = fileInput.value.files[0];
  // console.log(file);
  await splitFile(file, 64 * 1024, async (chunk: ArrayBuffer, num, totalChunks) => {
    return await uploadChunk(uuid, num, totalChunks, file.name, chunk);
  }, async (chunk, num) => await keychain.encrypt(chunk, num));
}
</script>

<style scoped>
.button-wrap {
    display: flex;
    gap: 10px;
}
</style>