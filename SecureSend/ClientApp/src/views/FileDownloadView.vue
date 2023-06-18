<script setup lang="ts">
import endpoints from '@/config/endpoints';
import type { SecureUploadDto } from '@/models/SecureUploadDto';

const props = defineProps<{
    secureUpload: SecureUploadDto;
}>();

</script>
<template>
    <h1>Download files</h1>
    <div>
        <div v-for="file in secureUpload.files" :key="(file.fileName as string)">
            <p>{{ file.fileName }}</p>
            <a :href="`${endpoints.download}?id=${secureUpload.secureUploadId}&fileName=${file.fileName}`" :download="file.fileName">Download</a>
        </div>
    </div>
</template>