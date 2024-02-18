import { computed, ref } from "vue";
import { useForm } from "vee-validate";

export interface IMappedFormValues {
  expiryDate: string;
  password: string;
  isPasswordRequired: boolean;
}

export function useFileUploadForm(isExpiryLimitSet: boolean) {
  const step = ref<number>(0);

  const stepZeroschema = {
    password(value: string) {
      if (!values.isPasswordRequired) return true;
      if (value) return true;
      return "Password is required.";
    },
  };

  const stepOneSchema = {
    expiryDate(value: string) {
      if (new Date(value) <= new Date())
        return "Expiry date must be later than today.";
      if (isExpiryLimitSet && !value) return "Expiry date is required";
      return true;
    },
  };

  const currentSchema = computed(() => {
    if (step.value === 0) return stepZeroschema;
    return stepOneSchema;
  });

  const getInitialValues = (): IMappedFormValues => {
    return {
      password: "",
      expiryDate: "",
      isPasswordRequired: false,
    };
  };

  const { handleSubmit, meta, resetForm, values } = useForm({
    validationSchema: currentSchema,
    initialValues: getInitialValues(),
    keepValuesOnUnmount: true,
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
