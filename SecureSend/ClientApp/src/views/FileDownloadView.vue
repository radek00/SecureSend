<script setup lang="ts">
import endpoints from '@/config/endpoints';
import type { SecureFileDto } from '@/models/SecureFileDto';
import type { SecureUploadDto } from '@/models/SecureUploadDto';
import { verifyHash } from '@/utils/pbkdfHash';
import { ref } from 'vue';

const props = defineProps<{
    secureUpload: SecureUploadDto;
    salt: string;
    passwordHash: string;
}>();

interface IWorkerInit {
    request: string;
    salt: Uint8Array;
    password: string;
    files: SecureFileDto[];
    id: string;
}

const password = ref<string>('')

const setUpWorker = () => {
    const decodedArray = new Uint8Array(atob(props.salt).split('').map((c) => c.charCodeAt(0)));
    navigator.serviceWorker.controller?.postMessage({
        request: 'init',
        salt: decodedArray,
        password: password.value,
        files: props.secureUpload.files,
        id: props.secureUpload.secureUploadId
    } as IWorkerInit)
}

const download = (url: string) => {
    setUpWorker();
    const anchor = document.createElement('a');
    anchor.href = url;
    document.body.appendChild(anchor);
    anchor.click()
}

const isPasswordValid = ref<boolean>();

const verifyPassword = async() => {
    isPasswordValid.value = await verifyHash(props.passwordHash, password.value);
}

</script>
<template>
    <h1>Download files</h1>

    <div v-if="isPasswordValid">
        <div v-for="file in secureUpload.files" :key="(file.fileName as string)">
            <p>{{ file.fileName }}</p>
            <a href="#" @click.prevent @click="download(`${endpoints.download}?id=${secureUpload.secureUploadId}&fileName=${file.fileName}`)">Download</a>
        </div>
    </div>
    <div v-else>
        <input v-model="password" name="password" type="password" placeholder="Password">
        <p v-show="isPasswordValid === false">Invalid password</p>
        <button @click="verifyPassword()">Unlock</button>
    </div>
</template>