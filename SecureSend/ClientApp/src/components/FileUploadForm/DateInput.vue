<script setup lang="ts">
import { useField } from 'vee-validate';

const props = defineProps<{
    name:  string;
}>();

const {errorMessage, value, meta} =  useField(() => props.name)
</script>

<template>
    <div>
        <label :for="props.name"></label>
        <input :name="props.name" type="date" v-model="value">
        <p v-if="meta.dirty">{{ errorMessage }}</p>
    </div>
</template>