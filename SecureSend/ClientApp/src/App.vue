<script setup lang="ts">
import { onMounted } from 'vue';


onMounted(async () => {
  console.log('mounted')
  const data = await fetch('/api/WeatherForecast');
  const response = await data.json();
  console.log(response);
})
</script>

<template>

</template>

