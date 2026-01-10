<template>
  <label
    v-if="label"
    :for="props.name"
    class="block mb-2 text-sm font-medium"
    :class="{
      'text-gray-400': disabled,
      'text-gray-900 dark:text-white': !disabled,
    }"
    >{{ props.label }}<slot name="label"></slot
  ></label>
  <input
    :id="props.name"
    :value="value"
    @input="
      $emit('update:modelValue', ($event.target as HTMLInputElement).value)
    "
    @input.once="isDirty = true"
    :name="props.name"
    v-bind="$attrs"
    :disabled="disabled"
    class="border sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 bg-gray-50 dark:bg-gray-700 border-gray-300 dark:border-gray-600 placeholder-gray-400 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500 disabled:cursor-not-allowed disabled:text-gray-400"
  />
  <ErrorMessage>
    <span v-show="isDirty && isValid === false" class="font-medium">{{
      errorMessage
    }}</span>
  </ErrorMessage>
</template>

<script setup lang="ts">
import ErrorMessage from "@/components/ErrorMessage.vue";
import { ref } from "vue";

const props = defineProps<{
  name: string;
  label?: string;
  value: unknown;
  isValid?: boolean;
  errorMessage?: string;
  disabled?: boolean;
}>();
defineEmits(["update:modelValue"]);

const isDirty = ref<boolean>(false);
</script>
