<script setup lang="ts">
import { useField } from 'vee-validate';
import ErrorMessage from '../ErrorMessage.vue';
const props = defineProps<{
    name:  string;
}>();

const {errorMessage, value} =  useField(() => props.name)
</script>
<template>
    <div class="mb-6">
        <label for="password" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Input encryption password</label>
        <input  :name="name" v-model="value" type="password" id="password" class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500">
        <ErrorMessage>
            <span class="font-medium">{{ errorMessage }}</span>
        </ErrorMessage>
    </div>    
</template>