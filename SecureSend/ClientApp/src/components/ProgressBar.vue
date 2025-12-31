<template>
  <div class="w-full rounded-full bg-gray-200 dark:bg-gray-700 mt-2">
    <div
      v-bind="$attrs"
      data-test="progress-bar"
      class="text-xs font-medium text-blue-100 text-center p-0.5 leading-none rounded-full"
      :class="classComputed"
      :style="{
        width: styleComputed,
      }"
    >
      {{ state[0] }}
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DownloadStateTuple } from "@/models/DownloadState";
import { UploadState, type UploadStateTuple } from "@/models/UploadStateTuple";
import { computed } from "vue";

const props = defineProps<{
  state: DownloadStateTuple | UploadStateTuple;
}>();

const classComputed = computed(() => {
  if (props.state[1] === UploadState.InProgress) {
    return "bg-blue-600";
  } else if (props.state[1] === UploadState.Completed) {
    return "bg-green-600";
  } else if (props.state[1] === UploadState.Failed) {
    return "bg-red-600";
  }
  return "bg-slate-700";
});

const styleComputed = computed(() => {
  if (
    props.state[1] === UploadState.InProgress ||
    props.state[1] === UploadState.NewFile
  ) {
    return props.state[0];
  } else {
    return "100%";
  }
});
</script>
