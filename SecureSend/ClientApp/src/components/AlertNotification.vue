<template>
  <Transition>
    <div
      v-if="isShown"
      ref="root"
      :class="`flex items-center p-4 mb-4 text-sm border rounded-lg bg-gray-800 ${textColor}`"
      role="alert"
    >
      <svg
        class="flex-shrink-0 inline w-4 h-4 mr-3"
        aria-hidden="true"
        xmlns="http://www.w3.org/2000/svg"
        fill="currentColor"
        viewBox="0 0 20 20"
      >
        <path
          d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5ZM9.5 4a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3ZM12 15H8a1 1 0 0 1 0-2h1v-3H8a1 1 0 0 1 0-2h2a1 1 0 0 1 1 1v4h1a1 1 0 0 1 0 2Z"
        />
      </svg>
      <span class="sr-only">Info</span>
      <div>
        <span class="font-medium">
          <slot></slot>
        </span>
      </div>
      <button @click="onCloseClick()">
        <CloseIcon
          class="ml-2 text-gray-400 border-solid hover:border-b-2 border-gray-400"
        ></CloseIcon>
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

defineEmits(["onCloseClick"]);

const root = ref();

const textColor = computed(() => {
  if (props.type === DialogType.Danger) return "text-red-400 border-red-800";
  if (props.type === DialogType.Success)
    return "text-green-400 border-green-800";
});

const isShown = ref<boolean>(false);

requestAnimationFrame(() => (isShown.value = true));

const onCloseClick = () => {
  const wrapper = root.value;
  render(null, wrapper);
  wrapper.parentElement.remove();
};

const props = defineProps<{
  type: DialogType;
}>();
</script>
