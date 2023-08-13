<template>
  <ol class="flex items-center w-full">
    <li
      :class="{
        'text-blue-500 after:border-blue-800 ': step === 0,
        'after:border-4 after:inline-block after:border-gray-700 ': step !== 0,
      }"
      class="flex w-full items-center after:content-[''] after:w-full after:h-1 after:border-b after:border-4 after:inline-block after:transition-background-color after:linear after:duration-1000"
    >
      <span
        class="flex items-center justify-center w-10 h-10 rounded-full lg:h-12 lg:w-12 shrink-0"
        :class="{
          'bg-blue-800 transition-background-color linear duration-1000  ':
            step === 0,
          'bg-gray-700 transition-background-color linear duration-1000':
            step !== 0,
        }"
      >
        <PasswordIcon></PasswordIcon>
      </span>
    </li>
    <li
      :class="{
        'text-blue-600 after:border-blue-800 transition-background-color linear duration-1000':
          step === 1,
        'after:border-gray-100 after:border-4 after:inline-block after:border-gray-700 transition-background-color linear duration-1000':
          step !== 1,
      }"
      class="flex w-full items-center after:content-[''] after:w-full after:h-1 after:border-b after:border-4 after:inline-block after:transition-background-color after:linear after:duration-1000"
    >
      <span
        class="flex items-center justify-center w-10 h-10 rounded-full lg:h-12 lg:w-12 shrink-0"
        :class="{
          'bg-blue-800 transition-background-color linear duration-1000':
            step === 1,
          'bg-gray-700 transition-background-color linear duration-1000':
            step !== 1,
        }"
      >
        <CalendarIcon></CalendarIcon>
      </span>
    </li>
    <li class="flex items-center">
      <span
        class="flex items-center justify-center w-10 h-10 rounded-full lg:h-12 lg:w-12 shrink-0"
        :class="{
          'bg-blue-800 transition-background-color linear duration-1000':
            step === 2,
          'bg-gray-700 transition-background-color linear duration-1000':
            step !== 2,
        }"
      >
        <UploadIconStep></UploadIconStep>
      </span>
    </li>
  </ol>
</template>

<script setup lang="ts">
import PasswordIcon from "@/assets/icons/PasswordIcon.vue";
import CalendarIcon from "@/assets/icons/CalendarIcon.vue";
import UploadIconStep from "@/assets/icons/UploadIconStep.vue";
defineProps<{
  step: number;
}>();
</script>
