<script setup lang="ts">
import { provide, ref } from "vue";
import { RouterView } from "vue-router";
import { useAlert } from "@/utils/composables/useAlert";

provide("isLoading", ref<boolean>(false));

const { openDanger, openSuccess } = useAlert();
</script>

<template>
  <div
    id="alert-container"
    class="absolute h-screen bottom-0 left-0 flex flex-col justify-end"
  ></div>
  <RouterView />
  <button @click="openDanger('Something went wrong')">Test alert</button>
  <button @click="openSuccess('Wow')">Test success</button>
  <!-- <FileUpload></FileUpload> -->
</template>
