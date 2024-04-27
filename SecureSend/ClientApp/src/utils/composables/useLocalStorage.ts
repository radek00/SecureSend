import { ref } from "vue";
import type { Ref } from "vue";

export function useLocalStorage<T>(key: string, initialValue?: T) {
  const storageItem = ref<T | undefined>(initialValue) as Ref<T>;
  function getItem(key: string) {
    const value = localStorage.getItem(key);
    if (value) {
      const item = JSON.parse(value);
      storageItem.value = item;
    }
  }

  function setItem() {
    localStorage.setItem(key, JSON.stringify(storageItem.value));
  }

  getItem(key);
  return { setItem, storageItem };
}
