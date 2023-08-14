import { type Ref, ref, shallowRef, watch } from "vue";

export interface UseDropZoneReturn {
  files: Ref<File[] | null>;
  isOverDropZone: Ref<boolean>;
}

export interface UseDropZoneOptions {
  onDrop?: (files: File[] | null, event: DragEvent) => void;
  onEnter?: (files: File[] | null, event: DragEvent) => void;
  onLeave?: (files: File[] | null, event: DragEvent) => void;
  onOver?: (files: File[] | null, event: DragEvent) => void;
}

export function useDropZone(
  target: Ref<HTMLElement | undefined>,
  options: UseDropZoneOptions
): UseDropZoneReturn {
  const isOverDropZone = ref(false);
  const files = shallowRef<File[] | null>(null);
  let counter = 0;

  const getFiles = (event: DragEvent) => {
    const list = Array.from(event.dataTransfer?.files ?? []);
    return (files.value = list.length === 0 ? null : list);
  };
  const registerListeners = () => {
    target.value?.addEventListener("dragenter", (event) => {
      event.preventDefault();
      counter += 1;
      isOverDropZone.value = true;
      options.onEnter?.(getFiles(event), event);
    });

    target.value?.addEventListener("dragover", (event) => {
      event.preventDefault();
      options.onOver?.(getFiles(event), event);
    });
    target.value?.addEventListener("dragleave", (event) => {
      event.preventDefault();
      counter -= 1;
      if (counter === 0) isOverDropZone.value = false;
      options.onLeave?.(getFiles(event), event);
    });
    target.value?.addEventListener("drop", (event) => {
      event.preventDefault();
      counter = 0;
      isOverDropZone.value = false;
      options.onDrop?.(getFiles(event), event);
    });
  };

  watch(target, (newVal) => {
    if (newVal) {
      registerListeners();
    }
  });
  return {
    files,
    isOverDropZone,
  };
}
