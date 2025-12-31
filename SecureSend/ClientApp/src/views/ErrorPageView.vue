<template>
  <section class="bg-white dark:bg-gray-900">
    <div class="py-8 px-4 mx-auto max-w-(--breakpoint-xl) lg:py-16 lg:px-6">
      <div class="mx-auto max-w-(--breakpoint-sm) text-center">
        <h1
          class="mb-4 text-7xl tracking-tight font-extrabold lg:text-9xl text-primary-600 text-primary-500"
        >
          {{ errorCode }}
        </h1>
        <p
          class="mb-4 text-3xl tracking-tight font-bold md:text-4xl text-gray-900 dark:text-white"
        >
          {{ heading }}
        </p>
        <p class="mb-4 text-lg font-light text-gray-600 dark:text-gray-400">
          {{ bottomText }}
        </p>
        <router-link
          to="/"
          class="inline-flex text-white bg-primary-600 hover:bg-primary-800 focus:ring-4 focus:outline-hidden focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center focus:ring-primary-900 my-4"
          >Back to Homepage</router-link
        >
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    errorCode: string;
  }>(),
  {
    errorCode: "404",
  }
);

const determineText = () => {
  if (props.errorCode === "410")
    return {
      heading: "Upload expired.",
      bottomText: "Sorry, your upload has expired. Try creating a new one",
    };
  return {
    heading: "Something's missing.",
    bottomText: "Either the link is corrupted or the upload does not exist.",
  };
};

const { heading, bottomText } = determineText();
</script>
