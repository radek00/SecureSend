import { createRouter, createWebHistory } from 'vue-router'
//import FileUpload from '../views/FileUpload'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // {
    //   path: '/',
    //   name: 'home',
    //   component: FileUpload
    // },

  ]
})

export default router
