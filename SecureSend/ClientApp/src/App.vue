<script setup lang="ts">
import { RouterView } from "vue-router";
</script>

<template>
  <div
    id="alert-container"
    class="absolute z-50 h-fit top-1 left-1 flex flex-col"
  ></div>
  <RouterView v-slot="{ Component, route }">
    <transition name="fade" mode="out-in">
      <suspense>
          <component v-if="Component" :is="Component" :key="route.path"></component>
      </suspense>
    </transition>
  </RouterView>
</template>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
