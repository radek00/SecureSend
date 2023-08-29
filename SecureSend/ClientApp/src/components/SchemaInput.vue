<script setup lang="ts">
import { useField } from "vee-validate";
import SimpleInput from "@/components/SimpleInput.vue";
const props = defineProps<{
  name: string;
  disabled?: boolean;
}>();

const { errorMessage, value, meta } = useField(() => props.name);
</script>
<template>
  <div class="mb-6">
    <SimpleInput
      :value="value"
      v-model="value"
      :name="name"
      v-bind="$attrs"
      :errorMessage="errorMessage"
      :isValid="meta.valid"
      :disabled="disabled"
    ></SimpleInput>
  </div>
</template>
