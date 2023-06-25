<script setup lang="ts">
import { useField } from 'vee-validate';

const props = defineProps<{
    name:  string;
}>();

const {errorMessage, value} =  useField(() => props.name)
</script>
<template>
    <label for="password">Input encryption password</label>
    <input :name="name" v-model="value"  type="text">
    <p>{{ errorMessage }}</p>
</template>