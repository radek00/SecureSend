import { computed, onMounted, ref } from "vue";
import type { Ref, ComputedRef } from "vue";
import { UploadLimitsService } from "@/services/UploadLimitsService";
import type { UploadStateTuple } from "@/models/UploadStateTuple";

export interface IUseFileLimits {
  totalSize: ComputedRef<number>;
  sizeLimit: Ref<number>;
  isLimitExceeded: ComputedRef<boolean>;
  dateLimit: Ref<string>;
}
export function useFileLimits(
  files: Ref<Map<File, UploadStateTuple>>
): IUseFileLimits {
  const sizeLimit = ref<number>(0);
  const dateLimit = ref<string>("");
  onMounted(async () => {
    const limits = await UploadLimitsService.getUploadLimits();

    sizeLimit.value = limits.singleUploadLimitInGB;

    if (limits.maxExpirationInDays > 0) {
      const currentDate = new Date();

      currentDate.setDate(currentDate.getDate() + limits.maxExpirationInDays);

      dateLimit.value =
        currentDate.getFullYear() +
        "-" +
        ("0" + (currentDate.getMonth() + 1)).slice(-2) +
        "-" +
        ("0" + currentDate.getDate()).slice(-2);
    }
  });

  const totalSize = computed(() => {
    if (sizeLimit.value > 0) {
      const sizeInBytes = [...files.value.keys()].reduce((acc, current) => {
        return (acc += current.size);
      }, 0);

      return Math.round((sizeInBytes / (1024 * 1024 * 1024)) * 10) / 10;
    }
    return 0;
  });

  const isLimitExceeded = computed(() => totalSize.value > sizeLimit.value);

  return { totalSize, sizeLimit, isLimitExceeded, dateLimit };
}
