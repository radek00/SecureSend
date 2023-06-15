<script setup lang="ts">

defineEmits(['onFielsChange']);

defineProps<{
    files: Map<File, number>;
}>();


</script>

<template>
    <div>
        <input ref="fileInput" name="files" type="file" multiple @change="$emit('onFielsChange', $event)">
    </div>
    <div class="button-wrap">
        <div v-for="[key, value] in files">
            {{ key.name }} = {{ value }}
        </div>
    </div>
</template>

<style>
.button-wrap {
    display: flex;
    flex-direction: column;
    gap: 10px;
}
</style>