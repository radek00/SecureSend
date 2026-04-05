import { InvalidPasswordError } from "@/models/errors/ResponseErrors";
import { ref } from "vue";
import { SecureSendService } from "@/services/SecureSendService";

export function useDownloadForm() {
  const isPasswordValid = ref<boolean>();
  const password = ref<string>("");
  const verifyPassword = async (uploadId: string) => {
    try {
      const upload = await SecureSendService.viewSecureUpload({
        id: uploadId,
        password: password.value,
      });
      isPasswordValid.value = true;
      return upload;
    } catch (err: unknown) {
      if (err instanceof InvalidPasswordError) isPasswordValid.value = false;
      else throw err;
    }
  };
  return { verifyPassword, password, isPasswordValid };
}
