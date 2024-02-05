import { UploadExpiredError } from "@/models/errors/ResponseErrors";
import { SecureSendService } from "@/services/SecureSendService";
import { createRouter, createWebHistory } from "vue-router";
import type { RouteLocationNormalized } from "vue-router";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";

const FileUploadView = () => import("@/views/FileUploadView/FileUploadView.vue");
const FileDownloadView = () => import("@/views/FileDownloadView.vue");
const ErrorPageView = () => import("@/views/ErrorPageView.vue");
const LandingView = () => import("@/views/LandingView.vue");

const routes = [
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
    beforeEnter: async (to: RouteLocationNormalized) => {
      try {
        (to.params.verifyUploadResponse as unknown as UploadVerifyResponseDTO) =
          await SecureSendService.verifySecureUpload(to.params.id as string);
        const keys = to.hash.split("_");
        to.params.b64Key = keys[0].slice(1);
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
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes,
});

export { routes };
export default router;
