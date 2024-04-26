export function useLocalStorage() {
  function getItem<T>(key: string): T | null {
    const value = localStorage.getItem(key);
    if (value) {
      return JSON.parse(value);
    }
    return null;
  }

  function setItem<T>(key: string, value: T) {
    localStorage.setItem(key, JSON.stringify(value));
  }
  return { getItem, setItem };
}
