<template>
  <div>
    <h2>Сравнение на депозити</h2>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary"></div>
    </div>

    <div v-else-if="deposits.length === 0" class="alert alert-info">
      <p>Няма избрани депозити за сравнение.</p>
      <p>Отидете в <router-link to="/catalog">каталога</router-link> и изберете до 3 депозита.</p>
    </div>

    <div v-else>
      <div class="table-responsive">
        <table class="table table-bordered table-striped">
          <thead class="table-primary">
            <tr>
              <th style="width: 20%">Характеристика</th>
              <th v-for="d in deposits" :key="d.id" :style="{ width: `${80 / deposits.length}%` }">
                {{ d.name }}<br>
                <small class="text-muted">{{ d.bank.name }}</small>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <th>Лихвен процент</th>
              <td v-for="d in deposits" :key="d.id" :class="{ 'table-success': d.interestRate === maxRate }">
                <strong>{{ d.interestRate.toFixed(2) }}%</strong>
                <span v-if="d.interestRate === maxRate" class="badge bg-success ms-1">Най-висока</span>
              </td>
            </tr>
            <tr>
              <th>Срок</th>
              <td v-for="d in deposits" :key="d.id">{{ d.termMonths }} месеца</td>
            </tr>
            <tr>
              <th>Валута</th>
              <td v-for="d in deposits" :key="d.id">{{ d.currency }}</td>
            </tr>
            <tr>
              <th>Минимална сума</th>
              <td v-for="d in deposits" :key="d.id" :class="{ 'table-success': d.minAmount === minAmount }">
                {{ d.minAmount.toLocaleString() }} {{ d.currency }}
                <span v-if="d.minAmount === minAmount" class="badge bg-success ms-1">Най-ниска</span>
              </td>
            </tr>
            <tr>
              <th>Капитализация</th>
              <td v-for="d in deposits" :key="d.id">
                <span :class="d.hasCapitalization ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ d.hasCapitalization ? 'Да' : 'Не' }}
                </span>
              </td>
            </tr>
            <tr>
              <th>Довнасяне</th>
              <td v-for="d in deposits" :key="d.id">
                <span :class="d.allowsAdditionalDeposits ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ d.allowsAdditionalDeposits ? 'Да' : 'Не' }}
                </span>
              </td>
            </tr>
            <tr>
              <th>Частично теглене</th>
              <td v-for="d in deposits" :key="d.id">
                <span :class="d.allowsPartialWithdrawal ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ d.allowsPartialWithdrawal ? 'Да' : 'Не' }}
                </span>
              </td>
            </tr>
            <tr>
              <th>Автоподновяване</th>
              <td v-for="d in deposits" :key="d.id">
                <span :class="d.autoRenewal ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ d.autoRenewal ? 'Да' : 'Не' }}
                </span>
              </td>
            </tr>
            <tr>
              <th>Детайли</th>
              <td v-for="d in deposits" :key="d.id">
                <router-link :to="`/deposit/${d.id}`" class="btn btn-sm btn-primary">Виж</router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="mt-3">
        <router-link to="/catalog" class="btn btn-outline-secondary">Назад</router-link>
        <button @click="clearCompare" class="btn btn-outline-danger ms-2">Изчисти</button>
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * Compare View
 *
 * Side-by-side comparison of selected deposits:
 * - Loads deposit IDs from localStorage
 * - Displays comparison table with all features and terms
 * - Highlights best values (highest interest rate, lowest minimum amount)
 * - Allows clearing the comparison list
 */
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'

const deposits = ref([])
const loading = ref(true)

/**
 * Finds the highest interest rate among compared deposits
 */
const maxRate = computed(() => Math.max(...deposits.value.map(d => d.interestRate)))

/**
 * Finds the lowest minimum amount among compared deposits
 */
const minAmount = computed(() => Math.min(...deposits.value.map(d => d.minAmount)))

/**
 * Loads deposits from the comparison list stored in localStorage
 */
async function loadDeposits() {
  const ids = JSON.parse(localStorage.getItem('compareIds') || '[]')
  if (ids.length > 0) {
    const results = await Promise.all(ids.map(id => api.getDeposit(id).catch(() => null)))
    deposits.value = results.filter(d => d !== null)
  }
  loading.value = false
}

/**
 * Clears the comparison list and reloads the page
 */
function clearCompare() {
  localStorage.removeItem('compareIds')
  deposits.value = []
}

onMounted(loadDeposits)
</script>
