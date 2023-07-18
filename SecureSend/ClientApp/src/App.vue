<script setup lang="ts">
import { RouterView } from "vue-router";
</script>

<template>
  <div
    id="alert-container"
    class="absolute z-50 h-fit top-1 left-1 flex flex-col"
  ></div>
  <RouterView />
</template>
