import { computed, type Ref, ref } from "vue";
import { useForm } from "vee-validate";
import { toUTCDate } from "@/utils/utils";

export interface IMappedFormValues {
  expiryDate: string;
  password: string;
  isPasswordRequired: boolean[];
}

export function useFileUploadForm(dateLimit: Ref<string>) {
  const step = ref<number>(0);
  const currentDate = toUTCDate(new Date());

  const stepZeroschema = {
    password(value: string) {
      //vee-validate thinks that checkboxes are an array since there are two rendered at the same time (mobile and desktop)
      if (
        values.isPasswordRequired[values.isPasswordRequired.length - 1] ===
        false
      )
        return true;
      if (value) return true;
      return "Password is required.";
    },
  };

  const stepOneSchema = {
    expiryDate(value: string) {
      if (dateLimit.value !== "" && !value) return "Expiry date is required";
      const checkedDate = new Date(value);
      if (checkedDate <= currentDate)
        return "Expiry date must be later than today.";
      if (dateLimit.value !== "" && checkedDate > new Date(dateLimit.value))
        return `Max allowed expiration date is: ${dateLimit.value}`;
      return true;
    },
  };

  const currentSchema = computed(() => {
    //todo: better way to determine desktop
    if (window.innerWidth >= 1024)
      return { ...stepZeroschema, ...stepOneSchema };
    if (step.value === 0) return stepZeroschema;
    return stepOneSchema;
  });

  const getInitialValues = (): IMappedFormValues => {
    const date = toUTCDate(new Date());
    date.setDate(date.getDate() + 1);
    return {
      password: "",
      expiryDate: date.toISOString().split("T")[0]!,
      isPasswordRequired: [false],
    };
  };

  const { handleSubmit, meta, resetForm, values } = useForm({
    validationSchema: currentSchema,
    initialValues: getInitialValues(),
    validateOnMount: false,
  });

  const resetUploadForm = () => {
    resetForm({ values: getInitialValues() });
    step.value = 0;
  };

  return {
    handleSubmit,
    meta,
    resetUploadForm,
    values,
    getInitialValues,
    step,
  };
}
