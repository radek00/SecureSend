import { InvalidPasswordError } from "@/models/errors/ResponseErrors";
import { inject, type Ref, ref } from "vue";
import { SecureSendService } from "@/services/SecureSendService";

export function useDownloadForm() {
  const isLoading = inject<Ref<boolean>>("isLoading");
  const isPasswordValid = ref<boolean>();
  const password = ref<string>("");
  const verifyPassword = async (uploadId: string) => {
    isLoading!.value = true;
    try {
      const upload = await SecureSendService.viewSecureUpload({
        id: uploadId,
        password: password.value,
      });
      isPasswordValid.value = true;
      isLoading!.value = false;
      return upload;
    } catch (err: unknown) {
      isLoading!.value = false;
      if (err instanceof InvalidPasswordError) isPasswordValid.value = false;
      else throw err;
    }
  };
  return { verifyPassword, password, isPasswordValid };
}
