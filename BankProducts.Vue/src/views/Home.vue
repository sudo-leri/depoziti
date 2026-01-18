<template>
  <div>
    <div class="bg-light p-5 rounded mb-4">
      <h1 class="display-4">Банкови Продукти</h1>
      <p class="lead">Сравнете депозити от водещи банки в България.</p>
      <hr class="my-4">
      <p>Разгледайте нашия каталог с актуални депозитни продукти.</p>
      <router-link to="/catalog" class="btn btn-primary btn-lg">Към каталога</router-link>
    </div>

    <h2 class="mb-4">Депозити с най-висока лихва</h2>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Зареждане...</span>
      </div>
    </div>

    <div v-else class="row">
      <div v-for="deposit in topDeposits" :key="deposit.id" class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-header d-flex justify-content-between align-items-center">
            <span>{{ deposit.bank.name }}</span>
            <span class="badge bg-success fs-6">{{ deposit.interestRate.toFixed(2) }}%</span>
          </div>
          <div class="card-body">
            <h5 class="card-title">
              <router-link :to="`/deposit/${deposit.id}`" class="text-decoration-none text-dark">
                {{ deposit.name }}
              </router-link>
            </h5>
            <ul class="list-unstyled">
              <li><strong>Срок:</strong> {{ deposit.termMonths }} месеца</li>
              <li><strong>Минимална сума:</strong> {{ deposit.minAmount.toLocaleString() }} {{ deposit.currency }}</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * Home View
 *
 * Landing page displaying:
 * - Welcome message
 * - Top 6 deposits sorted by interest rate (highest first)
 * - Quick access to detailed deposit information
 */
import { ref, onMounted } from 'vue'
import api from '../services/api'

const topDeposits = ref([])
const loading = ref(true)

/**
 * On mount, fetches all deposits sorted by interest rate
 * and displays the top 6
 */
onMounted(async () => {
  try {
    const deposits = await api.getDeposits({ sortBy: 'interest' })
    topDeposits.value = deposits.slice(0, 6)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.card-title a {
  transition: color 0.2s;
}

.card-title a:hover {
  color: var(--bs-primary) !important;
}
</style>
