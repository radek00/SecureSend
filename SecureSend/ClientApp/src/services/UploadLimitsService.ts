import { fetchWrapper } from "@/utils/fetchWrapper";
import endpoints from "@/config/endpoints";
import type { UploadSizeLimitsDto } from "@/models/UploadSizeLimitsDto";

export class UploadLimitsService {
  static getUploadLimits = async (): Promise<UploadSizeLimitsDto> => {
    return fetchWrapper.get(endpoints.uploadLimits);
  };
}
