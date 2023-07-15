<script setup lang="ts">
import { provide, ref } from "vue";
import { RouterView } from "vue-router";

provide("isLoading", ref<boolean>(false));
</script>

<template>
  <RouterView />
  <!-- <FileUpload></FileUpload> -->
</template>
