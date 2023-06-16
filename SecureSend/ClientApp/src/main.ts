import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.config.errorHandler = (error) => {
    console.log('global error', error);
}

app.use(router)

app.mount('#app')
