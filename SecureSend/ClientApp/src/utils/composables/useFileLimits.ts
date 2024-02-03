import { computed, onBeforeMount, ref } from "vue";
import type { Ref, ComputedRef } from "vue";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import type { UploadStateTuple } from "@/models/UploadStateTuple";

export interface IUseFileLimits {
  totalSize: ComputedRef<number>;
  sizeLimit: Ref<number>;
  isLimitExceeded: ComputedRef<boolean>;
}
export function useFileLimits(
  files: Ref<Map<File, UploadStateTuple>>
): IUseFileLimits {
  const sizeLimit = ref<number>(0);
  onBeforeMount(async () => {
    sizeLimit.value = (
      await UploadLimitsService.getUploadLimits()
    ).singleUploadLimitInGB;
  });

  const totalSize = computed(() => {
    const sizeInBytes = [...files.value.keys()].reduce((acc, current) => {
      return (acc += current.size);
    }, 0);

    return Math.round((sizeInBytes / (1024 * 1024 * 1024)) * 10) / 10;
  });

  const isLimitExceeded = computed(() => totalSize.value > sizeLimit.value);

  return { totalSize, sizeLimit, isLimitExceeded };
}
