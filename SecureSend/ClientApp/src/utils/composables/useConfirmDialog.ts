import { type Ref, ref, computed, type ComputedRef } from "vue";

export type UseConfirmDialogRevealResult<C, D> =
  | {
      data?: C;
      isCanceled: false;
    }
  | {
      data?: D;
      isCanceled: true;
    };

export interface UseConfirmDialogReturn<RevealData, ConfirmData, CancelData> {
  isRevealed: ComputedRef<boolean>;
  reveal: (
    data?: RevealData
  ) => Promise<UseConfirmDialogRevealResult<ConfirmData, CancelData>>;
  confirm: (data?: ConfirmData) => void;
  cancel: (data?: CancelData) => void;
}

export function useConfirmDialog<ConfirmData, CancelData>(
  revealed: Ref<boolean> = ref(false)
) {
  let _resolve: (
    arg0: UseConfirmDialogRevealResult<ConfirmData, CancelData>
  ) => void;

  const reveal = () => {
    revealed.value = true;

    return new Promise<UseConfirmDialogRevealResult<ConfirmData, CancelData>>(
      (resolve) => {
        _resolve = resolve;
      }
    );
  };

  const confirm = (data: ConfirmData) => {
    revealed.value = false;

    _resolve({ data, isCanceled: false });
  };

  const cancel = (data?: CancelData) => {
    revealed.value = false;
    _resolve({ data, isCanceled: true });
  };

  return {
    isRevealed: computed(() => revealed.value),
    reveal,
    confirm,
    cancel,
  };
}
