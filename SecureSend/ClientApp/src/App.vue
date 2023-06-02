<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onMounted } from 'vue';
import FileUpload from './views/FileUpload.vue';

onMounted(async () => {
  console.log('mounted')
  const data = await fetch('/api/WeatherForecast');
  const response = await data.json();
  console.log(response);
})
</script>

<template>
  <!-- <RouterView /> -->
  <FileUpload></FileUpload>
</template>

