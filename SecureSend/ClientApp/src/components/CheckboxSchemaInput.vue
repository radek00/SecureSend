<script setup lang="ts">
import { useField } from "vee-validate";
import SimpleCheckboxInput from "@/components/SimpleCheckboxInput.vue";

const props = defineProps<{
  name: string;
  checkedValue?: unknown;
  uncheckedValue?: unknown;
}>();

const { errorMessage, value, meta, handleChange, checked } = useField(
  () => props.name,
  undefined,
  {
    type: "checkbox",
    checkedValue: props.checkedValue ?? true,
    uncheckedValue: props.uncheckedValue ?? false,
  }
);
</script>
<template>
  <div class="mt-3">
    <SimpleCheckboxInput
      :checked="checked"
      :value="value"
      @update:model-value="handleChange"
      :name="name"
      v-bind="$attrs"
      :errorMessage="errorMessage"
      :isValid="meta.valid"
    >
    </SimpleCheckboxInput>
  </div>
</template>
