import { createRouter, createWebHistory } from 'vue-router'
import Home from './views/Home.vue'
import Catalog from './views/Catalog.vue'
import Details from './views/Details.vue'
import Compare from './views/Compare.vue'

/**
 * Application route definitions
 */
const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/catalog', name: 'Catalog', component: Catalog },
  { path: '/deposit/:id', name: 'Details', component: Details },
  { path: '/compare', name: 'Compare', component: Compare }
]

/**
 * Vue Router instance configured with HTML5 history mode
 */
export default createRouter({
  history: createWebHistory(),
  routes
})
