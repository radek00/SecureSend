<template>
  <Transition>
    <div
      v-show="isShown"
      ref="root"
      :class="`flex items-center gap-2 w-fit p-3 mb-4 text-sm border rounded-lg bg-gray-800 ${textColor}`"
      role="alert"
    >
      <InfoIcon></InfoIcon>
      <span class="sr-only">Info</span>
      <span class="font-medium">
        <slot></slot>
      </span>
      <button
        @click="onCloseClick()"
        type="button"
        class="p-1 rounded-lg focus:ring-2 focus:ring-gray-300 inline-flex items-center justify-center h-6 w-6 text-gray-500 hover:text-white bg-gray-800 hover:bg-gray-700"
        data-dismiss-target="#toast-success"
        aria-label="Close"
      >
        <span class="sr-only">Close</span>
        <CloseIcon class="w-3 h-3"></CloseIcon>
      </button>
    </div>
  </Transition>
</template>

<style scoped>
.v-enter-active,
.v-leave-active {
  transition: opacity 1s ease;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}
</style>

<script setup lang="ts">
import { computed, ref, render } from "vue";
import CloseIcon from "@/assets/icons/CloseIcon.vue";
import { DialogType } from "../utils/composables/useAlert";
import InfoIcon from "@/assets/icons/InfoIcon.vue";

defineEmits(["onCloseClick"]);

const root = ref();

const textColor = computed(() => {
  if (props.type === DialogType.Danger) return "text-red-400 border-red-800";
  if (props.type === DialogType.Success)
    return "text-green-400 border-green-800";
  return "";
});

const isShown = ref<boolean>(false);

requestAnimationFrame(() => (isShown.value = true));

const timeout = setTimeout(() => onCloseClick(), 5000);

const onCloseClick = () => {
  clearInterval(timeout);
  isShown.value = false;
  setTimeout(() => {
    const wrapper = root.value;
    render(null, wrapper);
    wrapper.parentElement.remove();
  }, 1000);
};

const props = defineProps<{
  type: DialogType;
}>();
</script>
