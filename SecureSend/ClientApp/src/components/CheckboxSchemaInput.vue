<script setup lang="ts">
import { useField } from "vee-validate";
import SimpleCheckboxInput from "@/components/SimpleCheckboxInput.vue";

const props = defineProps<{
  name: string;
  checkedValue: unknown;
}>();

const { errorMessage, value, meta, handleChange } = useField(
  () => props.name,
  undefined,
  {
    type: "checkbox",
    checkedValue: props.checkedValue,
  }
);
</script>
<template>
  <div class="mb-6">
    <SimpleCheckboxInput
      @change="handleChange(checkedValue)"
      :value="value"
      v-model="value"
      :name="name"
      v-bind="$attrs"
      :errorMessage="errorMessage"
      :isValid="meta.valid"
    >
    </SimpleCheckboxInput>
  </div>
</template>
