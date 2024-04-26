import { ref } from "vue";

export function useToggle(initialValue = false) {
  const value = ref(initialValue);
  const toggle = () => {
    value.value = !value.value;
  };
  return { value, toggle };
}
