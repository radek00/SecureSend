<script setup lang="ts">
import { inject } from "vue";
import type { IUseFileLimits } from "@/utils/composables/useFileLimits";

const limits = inject<IUseFileLimits>("sizeLimits")!;
</script>

<template>
  <div
    v-if="limits.sizeLimit.value > 0"
    class="text-center"
    :class="{ 'text-red-500': limits.isLimitExceeded.value }"
  >
    Size limit: {{ limits.totalSize }}/{{ limits.sizeLimit }} GB
  </div>
</template>
