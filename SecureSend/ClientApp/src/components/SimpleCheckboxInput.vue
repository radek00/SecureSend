<template>
  <div class="flex items-center mb-4">
    <input
      :name="name"
      v-bind="$attrs"
      :checked="checked"
      @change="
        $emit('update:modelValue', ($event.target as HTMLInputElement).checked)
      "
      id="default-checkbox"
      type="checkbox"
      :value="value"
      class="w-4 h-4 text-blue-600 rounded-sm focus:ring-blue-500 ring-offset-gray-50 dark:ring-offset-gray-800 focus:ring-2 bg-gray-100 dark:bg-gray-700 border-gray-300 dark:border-gray-600"
    />
    <label
      v-if="label"
      :for="name"
      class="ml-2 text-sm font-medium text-gray-700 dark:text-gray-300"
      >{{ label }}</label
    >
  </div>
  <ErrorMessage>
    <span v-show="isValid === false" class="font-medium">{{
      errorMessage
    }}</span>
  </ErrorMessage>
</template>

<script setup lang="ts">
import ErrorMessage from "@/components/ErrorMessage.vue";

defineEmits(["update:modelValue"]);

defineProps<{
  name: string;
  label?: string;
  value: unknown;
  isValid?: boolean;
  checked?: boolean;
  errorMessage?: string;
}>();
</script>
