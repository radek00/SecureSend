import type { Directive } from "vue";

export const clickOutside: Directive = {
  beforeMount: function (element, binding) {
    element.clickOutsideEvent = function (event: Event) {
      if (
        !(element === event.target || event.composedPath().includes(element))
      ) {
        binding.value(event);
      }
    };
    document.body.addEventListener("click", element.clickOutsideEvent);
  },
  unmounted: function (element) {
    document.body.removeEventListener("click", element.clickOutsideEvent);
  },
};
