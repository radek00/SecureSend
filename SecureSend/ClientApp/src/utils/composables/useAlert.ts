import AlertNotificationVue from "@/components/AlertNotification.vue";
import { h, render, type VNode } from "vue";

export enum DialogType {
  Success,
  Danger,
}

function mountComponent(component: VNode) {
  const alertContainer = document.getElementById("alert-container");
  const alert = document.createElement("div");
  alertContainer!.appendChild(alert);
  render(component, alert);
}

export interface UseAlertReturn {
  openDanger: (message: string) => void;
  openSuccess: (message: string) => void;
}

export function useAlert(): UseAlertReturn {
  return {
    openDanger(message: string) {
      const component = h(
        AlertNotificationVue,
        { type: DialogType.Danger },
        { default: () => message }
      );
      // component.component.
      mountComponent(component);
    },
    openSuccess(message: string) {
      const component = h(
        AlertNotificationVue,
        { type: DialogType.Success },
        { default: () => message }
      );
      mountComponent(component);
    },
  };
}
