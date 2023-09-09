<template>
  <div class="flex items-center mb-4">
    <input
      :name="name"
      v-bind="$attrs"
      :checked="checked"
      @input="
        $emit('update:modelValue', ($event.target as HTMLInputElement).value)
      "
      id="default-checkbox"
      type="checkbox"
      :value="value"
      class="w-4 h-4 text-blue-600 rounded focus:ring-blue-600 ring-offset-gray-800 focus:ring-2 bg-gray-700 border-gray-600"
    />
    <label
      v-if="label"
      :for="name"
      class="ml-2 text-sm font-medium text-gray-300"
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
