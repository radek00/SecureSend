import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { SecureSendService } from "@/services/SecureSendService";
import { createRouter, createWebHistory } from "vue-router";

const FileUploadView = () => import("@/views/FileUploadView.vue");
const FileDownloadView = () => import("@/views/FileDownloadView.vue");

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: FileUploadView,
    },
    {
      path: "/download/:id",
      name: "fileDownload",
      component: FileDownloadView,
      props: true,
      beforeEnter: async (to, form) => {
        try {
          const upload = await SecureSendService.viewSecureUpload({
            id: to.params.id as string,
          });
          (to.params.secureUpload as unknown as SecureUploadDto) = upload;
          const keys = to.hash.split("_");
          (to.params.salt as unknown as Uint8Array) = new Uint8Array(
            atob(keys[0].slice(1))
              .split("")
              .map((c) => c.charCodeAt(0))
          );
          to.params.passwordHash = keys[1];
        } catch (error) {
          return { path: form.path };
        }
      },
    },
  ],
});

export default router;
