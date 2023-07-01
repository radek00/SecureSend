<script setup lang="ts">
import { useField } from "vee-validate";
import ErrorMessage from "../ErrorMessage.vue";
const props = defineProps<{
  name: string;
}>();

const { errorMessage, value, meta } = useField(() => props.name);
</script>

<template>
  <div class="mb-6">
    <label
      class="block mb-2 text-sm font-medium text-gray-900 dark:text-white"
      :for="props.name"
      >Choose optional expiry date</label
    >
    <input
      class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
      :name="props.name"
      type="date"
      v-model="value"
    />
    <ErrorMessage>
      <span class="font-medium">{{ errorMessage }}</span>
    </ErrorMessage>
  </div>
</template>
