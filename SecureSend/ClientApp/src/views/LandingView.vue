<template>
  <div
    class="max-w-5xl mx-auto pt-20 sm:pt-24 lg:pt-32 flex flex-col gap-4 items-center"
  >
    <h1
      class="font-extrabold text-4xl sm:text-5xl lg:text-6xl tracking-tight text-center"
    >
      Share your files securely.
    </h1>
    <p class="text-lg text-center max-w-3xl mx-auto text-slate-400">
      Secure Send lets you share files with end-to-end encryption. Set your
      upload to expire whenever you want and generate a link to access the
      upload.
    </p>
    <router-link
      to="/upload"
      class="text-white focus:ring-4 font-medium rounded-lg text-sm px-5 py-2.5 mr-2 mb-2 bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-blue-800"
    >
      Get started
    </router-link>
  </div>
</template>
