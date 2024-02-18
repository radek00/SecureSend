<template>
  <div class="relative">
    <p
      data-test="error-message"
      class="absolute right-0 mt-2 text-sm text-red-500"
    >
      <slot>
        <span class="font-medium">Oh, snapp!</span> Something went wrong.
      </slot>
    </p>
  </div>
</template>
