<script setup lang="ts">
import DropdownIcon from "@/assets/icons/DropdownIcon.vue";
import CopyIcon from "@/assets/icons/CopyIcon.vue";
</script>

<template>
  <div class="flex items-center justify-between mb-4">
    <h5 class="text-xl font-bold leading-none text-white">Upload history</h5>
  </div>
  <div class="overflow-auto max-h-40 lg:max-h-96">
    <ul role="list" class="divide-y divide-gray-700">
      <li class="py-3 sm:py-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <a
              class="text-sm underline cursor-pointer text-blue-500 hover:no-underline flex items-center gap-2 mt-2"
            >
              <DropdownIcon></DropdownIcon>
            </a>
          </div>
          <div class="flex-1 min-w-0 ms-4">
            <p class="text-sm font-medium truncate text-white">23.04.2024</p>
            <p class="text-sm truncate text-gray-400">5 files</p>
          </div>
          <div
            class="inline-flex items-center text-base font-semibold text-white"
          >
            <button
              class="text-gray-400 bg-gray-800 border-gray-600 hover:bg-gray-700 rounded-lg py-2 px-2.5 inline-flex items-center justify-center border"
            >
              <span id="default-message" class="inline-flex items-center">
                <CopyIcon></CopyIcon>
                <span class="text-xs font-semibold">Copy</span>
              </span>
            </button>
          </div>
        </div>
      </li>
      <li class="py-3 sm:py-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <a
              class="text-sm underline cursor-pointer text-blue-500 hover:no-underline flex items-center gap-2 mt-2"
            >
              <svg
                class="w-2.5 h-2.5"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 10 6"
              >
                <path
                  stroke="currentColor"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="m1 1 4 4 4-4"
                />
              </svg>
            </a>
          </div>
          <div class="flex-1 min-w-0 ms-4">
            <p class="text-sm font-medium truncate text-white">23.04.2024</p>
            <p class="text-sm truncate text-gray-400">5 files</p>
          </div>
          <div
            class="inline-flex items-center text-base font-semibold text-white"
          >
            <button
              class="text-gray-400 bg-gray-800 border-gray-600 hover:bg-gray-700 rounded-lg py-2 px-2.5 inline-flex items-center justify-center border"
            >
              <span id="default-message" class="inline-flex items-center">
                <svg
                  class="w-3 h-3 me-1.5"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="currentColor"
                  viewBox="0 0 18 20"
                >
                  <path
                    d="M16 1h-3.278A1.992 1.992 0 0 0 11 0H7a1.993 1.993 0 0 0-1.722 1H2a2 2 0 0 0-2 2v15a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2Zm-3 14H5a1 1 0 0 1 0-2h8a1 1 0 0 1 0 2Zm0-4H5a1 1 0 0 1 0-2h8a1 1 0 1 1 0 2Zm0-5H5a1 1 0 0 1 0-2h2V2h4v2h2a1 1 0 1 1 0 2Z"
                  />
                </svg>
                <span class="text-xs font-semibold">Copy</span>
              </span>
            </button>
          </div>
        </div>
      </li>
    </ul>
  </div>
</template>

<style scoped></style>
