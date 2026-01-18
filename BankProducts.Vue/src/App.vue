<template>
  <div>
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container">
        <router-link class="navbar-brand fw-bold" to="/">Банкови Продукти</router-link>
        <button 
          class="navbar-toggler" 
          type="button" 
          @click="toggleMenu"
          :aria-expanded="isMenuOpen"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div 
          class="navbar-collapse" 
          :class="{ 'collapse': !isMenuOpen, 'show': isMenuOpen }"
          id="navbarNav"
        >
          <ul class="navbar-nav">
            <li class="nav-item">
              <router-link class="nav-link" to="/" @click="closeMenu">Начало</router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/catalog" @click="closeMenu">Каталог</router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/compare" @click="closeMenu">
                Сравнение
                <span v-if="compareCount > 0" class="badge bg-warning text-dark">{{ compareCount }}</span>
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <main class="container py-4">
      <router-view @add-to-compare="addToCompare" :compare-ids="compareIds" />
    </main>

    <footer class="bg-light py-3 mt-5">
      <div class="container text-center text-muted">
        &copy; 2026 Банкови Продукти - Vue.js SPA
      </div>
    </footer>
  </div>
</template>

<script setup>
/**
 * App Component
 *
 * Root component managing:
 * - Navigation bar with links to all pages
 * - Compare feature with badge showing count of selected deposits
 * - Mobile-responsive menu toggle
 * - Deposit comparison list stored in localStorage (max 3 items)
 * - Global event handling for adding deposits to comparison
 */
import { ref, computed } from 'vue'

/**
 * Array of deposit IDs selected for comparison, persisted in localStorage
 */
const compareIds = ref(JSON.parse(localStorage.getItem('compareIds') || '[]'))

/**
 * Computed count of deposits in comparison list
 */
const compareCount = computed(() => compareIds.value.length)

/**
 * Mobile menu open/close state
 */
const isMenuOpen = ref(false)

/**
 * Adds a deposit to the comparison list
 * Maximum 3 deposits can be compared at once
 * @param {number} id - The deposit ID to add
 */
function addToCompare(id) {
  if (!compareIds.value.includes(id) && compareIds.value.length < 3) {
    compareIds.value.push(id)
    localStorage.setItem('compareIds', JSON.stringify(compareIds.value))
  }
}

/**
 * Toggles mobile navigation menu
 */
function toggleMenu() {
  isMenuOpen.value = !isMenuOpen.value
}

/**
 * Closes mobile navigation menu
 */
function closeMenu() {
  isMenuOpen.value = false
}
</script>

<style>
.router-link-active {
  font-weight: bold;
}

@media (max-width: 991.98px) {
  .navbar-collapse {
    margin-top: 1rem;
  }
  
  .navbar-collapse.collapse {
    display: none !important;
  }
  
  .navbar-collapse.show {
    display: block !important;
  }
}
</style>
