import { UploadExpiredError } from "@/models/errors/ResponseErrors";
import { SecureSendService } from "@/services/SecureSendService";
import { createRouter, createWebHistory } from "vue-router";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";

const FileUploadView = () => import("@/views/FileUploadView.vue");
const FileDownloadView = () => import("@/views/FileDownloadView.vue");
const ErrorPageView = () => import("@/views/ErrorPageView.vue");
const LandingView = () => import("@/views/LandingView.vue");

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: LandingView,
    },
    {
      path: "/upload",
      name: "upload",
      component: FileUploadView,
    },
    {
      path: "/error/:errorCode",
      name: "error",
      props: true,
      component: ErrorPageView,
    },
    {
      path: "/download/:id",
      name: "fileDownload",
      component: FileDownloadView,
      props: true,
      beforeEnter: async (to) => {
        try {
          (to.params
            .verifyUploadResponse as unknown as UploadVerifyResponseDTO) =
            await SecureSendService.verifySecureUpload(to.params.id as string);
          const keys = to.hash.split("_");
          (to.params.salt as unknown as Uint8Array) = new Uint8Array(
            atob(keys[0].slice(1))
              .split("")
              .map((c) => c.charCodeAt(0))
          );
          const isProtected = to.query["pass"] === "true";
          (to.params.isPasswordProtected as unknown as boolean) = isProtected;
          (to.params.masterKey as unknown as string | Uint8Array) = isProtected
            ? keys[1]
            : new Uint8Array(
                atob(keys[1])
                  .split("")
                  .map((c) => c.charCodeAt(0))
              );
        } catch (error) {
          if (error instanceof UploadExpiredError) {
            return {
              name: "error",
              params: {
                errorCode: "410",
              },
            };
          }
          return { name: "error", params: { errorCode: "404" } };
        }
      },
    },
  ],
});

export default router;
