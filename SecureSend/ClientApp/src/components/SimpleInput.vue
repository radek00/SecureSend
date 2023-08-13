<template>
  <label
    v-if="label"
    :for="props.name"
    class="block mb-2 text-sm font-medium text-white"
    >{{ props.label }}</label
  >
  <input
    :value="value"
    @input="
      $emit('update:modelValue', ($event.target as HTMLInputElement).value)
    "
    :name="props.name"
    v-bind="$attrs"
    class="border sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 bg-gray-700 border-gray-600 placeholder-gray-400 text-white focus:ring-blue-500 focus:border-blue-500"
  />
  <ErrorMessage>
    <span v-show="isValid === false" class="font-medium">{{
      errorMessage
    }}</span>
  </ErrorMessage>
</template>

<script setup lang="ts">
import ErrorMessage from "@/components/ErrorMessage.vue";

const props = defineProps<{
  name: string;
  label?: string;
  value: unknown;
  isValid?: boolean;
  errorMessage?: string;
}>();
defineEmits(["update:modelValue"]);
</script>
