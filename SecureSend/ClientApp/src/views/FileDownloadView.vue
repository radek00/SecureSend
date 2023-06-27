<script setup lang="ts">
import endpoints from '@/config/endpoints';
import type { SecureFileDto } from '@/models/SecureFileDto';
import type { SecureUploadDto } from '@/models/SecureUploadDto';
import { verifyHash } from '@/utils/pbkdfHash';
import { ref } from 'vue';

const props = defineProps<{
    secureUpload: SecureUploadDto;
    salt: Uint8Array;
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
    navigator.serviceWorker.controller?.postMessage({
        request: 'init',
        salt: props.salt,
        password: password.value,
    } as IWorkerInit)
}

const download = (url: string) => {
    const anchor = document.createElement('a');
    anchor.href = url;
    document.body.appendChild(anchor);
    anchor.click()
}

const isPasswordValid = ref<boolean>();

const verifyPassword = async() => {
    isPasswordValid.value = await verifyHash(props.passwordHash, password.value);
    if(isPasswordValid.value) setUpWorker();
}

</script>
<template>
    <h1>Download files</h1>

    <div v-if="isPasswordValid" >
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