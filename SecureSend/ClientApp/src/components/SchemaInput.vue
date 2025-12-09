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
  <div>
    <SimpleInput
      :value="value"
      v-model="value"
      :name="name"
      v-bind="$attrs"
      :errorMessage="errorMessage"
      :isValid="meta.valid"
      :disabled="disabled"
    >
      <template #label>
        <slot name="label"></slot>
      </template>
    </SimpleInput>
  </div>
</template>
