<template>
  <div class="row">
    <!-- Filters -->
    <div class="col-md-3">
      <div class="card mb-4">
        <div class="card-header bg-primary text-white">
          <h5 class="mb-0">Филтри</h5>
        </div>
        <div class="card-body">
          <div class="mb-3">
            <label class="form-label">Банка</label>
            <select v-model="filters.bankId" class="form-select" @change="loadDeposits">
              <option value="">Всички банки</option>
              <option v-for="bank in banks" :key="bank.id" :value="bank.id">{{ bank.name }}</option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Валута</label>
            <select v-model="filters.currency" class="form-select" @change="loadDeposits">
              <option value="">Всички валути</option>
              <option value="BGN">BGN</option>
              <option value="EUR">EUR</option>
              <option value="USD">USD</option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Срок (месеци)</label>
            <div class="row g-2">
              <div class="col-6">
                <input type="number" v-model="filters.minTerm" class="form-control" placeholder="От" @change="loadDeposits">
              </div>
              <div class="col-6">
                <input type="number" v-model="filters.maxTerm" class="form-control" placeholder="До" @change="loadDeposits">
              </div>
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label">Минимална сума</label>
            <input type="number" v-model="filters.minAmount" class="form-control" @change="loadDeposits">
          </div>

          <div class="mb-3">
            <label class="form-label">Сортиране</label>
            <select v-model="filters.sortBy" class="form-select" @change="loadDeposits">
              <option value="interest">По лихва</option>
              <option value="term">По срок</option>
              <option value="minamount">По минимална сума</option>
            </select>
          </div>

          <button @click="clearFilters" class="btn btn-outline-secondary w-100">Изчисти</button>
        </div>
      </div>
    </div>

    <!-- Deposits List -->
    <div class="col-md-9">
      <h2>Каталог депозити</h2>
      <p class="text-muted">Намерени: {{ deposits.length }} депозита</p>

      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary"></div>
      </div>

      <div v-else class="row">
        <div v-for="deposit in deposits" :key="deposit.id" class="col-md-6 mb-4">
          <div class="card h-100">
            <div class="card-header d-flex justify-content-between align-items-center">
              <strong>{{ deposit.bank.name }}</strong>
              <span class="badge bg-success fs-6">{{ deposit.interestRate.toFixed(2) }}%</span>
            </div>
            <div class="card-body">
              <h5 class="card-title">
                <router-link :to="`/deposit/${deposit.id}`" class="text-decoration-none text-dark">
                  {{ deposit.name }}
                </router-link>
              </h5>
              <p class="card-text text-muted">{{ deposit.description }}</p>
              <ul class="list-unstyled">
                <li><strong>Срок:</strong> {{ deposit.termMonths }} месеца</li>
                <li><strong>Минимална сума:</strong> {{ deposit.minAmount.toLocaleString() }} {{ deposit.currency }}</li>
              </ul>
              <div class="mb-2">
                <span v-if="deposit.hasCapitalization" class="badge bg-info me-1">Капитализация</span>
                <span v-if="deposit.allowsAdditionalDeposits" class="badge bg-info me-1">Довнасяне</span>
                <span v-if="deposit.allowsPartialWithdrawal" class="badge bg-info me-1">Частично теглене</span>
                <span v-if="deposit.autoRenewal" class="badge bg-info me-1">Автоподновяване</span>
              </div>
            </div>
            <div class="card-footer">
              <button
                @click="$emit('add-to-compare', deposit.id)"
                class="btn btn-outline-secondary btn-sm"
                :disabled="compareIds.includes(deposit.id)"
              >
                {{ compareIds.includes(deposit.id) ? 'Добавен' : '+ Сравни' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * Deposit Catalog View
 *
 * Main catalog page with advanced filtering and sorting capabilities:
 * - Filter by bank, currency, term range, and minimum amount
 * - Sort by interest rate, term, minimum amount, or bank name
 * - Add deposits to comparison list
 * - Displays deposit cards with key information and features
 */
import { ref, reactive, onMounted } from 'vue'
import api from '../services/api'

defineProps(['compareIds'])
defineEmits(['add-to-compare'])

const deposits = ref([])
const banks = ref([])
const loading = ref(true)

/**
 * Reactive filter object for deposit search
 */
const filters = reactive({
  bankId: '',
  currency: '',
  minTerm: '',
  maxTerm: '',
  minAmount: '',
  sortBy: 'interest'
})

/**
 * Loads deposits from API with current filter settings
 */
async function loadDeposits() {
  loading.value = true
  try {
    deposits.value = await api.getDeposits(filters)
  } finally {
    loading.value = false
  }
}

/**
 * Clears all filters and reloads deposits with defaults
 */
function clearFilters() {
  Object.keys(filters).forEach(key => filters[key] = key === 'sortBy' ? 'interest' : '')
  loadDeposits()
}

onMounted(async () => {
  banks.value = await api.getBanks()
  await loadDeposits()
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
