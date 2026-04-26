<script setup lang="ts">
import { inject, type Ref } from "vue";
import { RouterView } from "vue-router";

const isWorkerLoading = inject<Ref<boolean>>("isWorkerLoading");
</script>

<template>
  <div
    id="alert-container"
    class="absolute z-50 h-fit top-1 left-1 flex flex-col"
  ></div>
  <p
    v-if="isWorkerLoading"
    class="flex h-full items-center justify-center text-4xl"
  >
    Loading...
  </p>
  <RouterView v-else v-slot="{ Component, route }">
    <transition name="fade" mode="out-in">
      <suspense>
        <component
          v-if="Component"
          :is="Component"
          :key="route.path"
        ></component>
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
