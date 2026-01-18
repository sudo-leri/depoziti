<template>
  <div v-if="loading" class="text-center py-5">
    <div class="spinner-border text-primary"></div>
  </div>

  <div v-else-if="deposit">
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/catalog">Каталог</router-link></li>
        <li class="breadcrumb-item active">{{ deposit.name }}</li>
      </ol>
    </nav>

    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header bg-primary text-white">
            <h4 class="mb-0">{{ deposit.name }}</h4>
          </div>
          <div class="card-body">
            <h5 class="text-muted">{{ deposit.bank.name }}</h5>
            <hr>
            <p>{{ deposit.description }}</p>

            <div class="row mt-4">
              <div class="col-md-6">
                <h5>Основни условия</h5>
                <table class="table">
                  <tbody>
                    <tr>
                      <th>Лихвен процент</th>
                      <td><span class="badge bg-success fs-5">{{ deposit.interestRate.toFixed(2) }}%</span></td>
                    </tr>
                    <tr>
                      <th>Срок</th>
                      <td>{{ deposit.termMonths }} месеца</td>
                    </tr>
                    <tr>
                      <th>Валута</th>
                      <td>{{ deposit.currency }}</td>
                    </tr>
                    <tr>
                      <th>Минимална сума</th>
                      <td>{{ deposit.minAmount.toLocaleString() }} {{ deposit.currency }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="col-md-6">
                <h5>Характеристики</h5>
                <ul class="list-group">
                  <li class="list-group-item d-flex justify-content-between">
                    Капитализация
                    <span :class="deposit.hasCapitalization ? 'badge bg-success' : 'badge bg-secondary'">
                      {{ deposit.hasCapitalization ? 'Да' : 'Не' }}
                    </span>
                  </li>
                  <li class="list-group-item d-flex justify-content-between">
                    Довнасяне
                    <span :class="deposit.allowsAdditionalDeposits ? 'badge bg-success' : 'badge bg-secondary'">
                      {{ deposit.allowsAdditionalDeposits ? 'Да' : 'Не' }}
                    </span>
                  </li>
                  <li class="list-group-item d-flex justify-content-between">
                    Частично теглене
                    <span :class="deposit.allowsPartialWithdrawal ? 'badge bg-success' : 'badge bg-secondary'">
                      {{ deposit.allowsPartialWithdrawal ? 'Да' : 'Не' }}
                    </span>
                  </li>
                  <li class="list-group-item d-flex justify-content-between">
                    Автоподновяване
                    <span :class="deposit.autoRenewal ? 'badge bg-success' : 'badge bg-secondary'">
                      {{ deposit.autoRenewal ? 'Да' : 'Не' }}
                    </span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
          <div class="card-footer">
            <button @click="showPaymentPlan = true" class="btn btn-primary">
              Изчисли
            </button>
            <router-link to="/catalog" class="btn btn-outline-secondary ms-2">Назад</router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- Модално прозорче за разплащателен план -->
    <div v-if="showPaymentPlan" class="modal fade show" style="display: block;" tabindex="-1">
      <div class="modal-dialog modal-lg modal-dialog-scrollable">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Разплащателен план - {{ deposit.name }}</h5>
            <button type="button" class="btn-close" @click="showPaymentPlan = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Сума на депозита ({{ deposit.currency }})</label>
              <input type="number" v-model.number="planAmount" class="form-control" :min="deposit.minAmount">
            </div>
            
            <div class="table-responsive">
              <table class="table table-striped table-hover">
                <thead class="table-primary">
                  <tr>
                    <th>Месец</th>
                    <th>Начална сума</th>
                    <th>Месечна лихва</th>
                    <th>Данъци (10%)</th>
                    <th>Нетно изплатени</th>
                    <th>ЕГЛ (%)</th>
                    <th>Крайна сума</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(payment, index) in paymentPlan" :key="index">
                    <td>{{ index + 1 }}</td>
                    <td>{{ payment.startAmount.toFixed(2) }} {{ deposit.currency }}</td>
                    <td>{{ payment.monthlyInterest.toFixed(2) }} {{ deposit.currency }}</td>
                    <td>{{ payment.tax.toFixed(2) }} {{ deposit.currency }}</td>
                    <td>{{ payment.netInterest.toFixed(2) }} {{ deposit.currency }}</td>
                    <td>{{ payment.egl.toFixed(2) }}%</td>
                    <td><strong>{{ payment.endAmount.toFixed(2) }} {{ deposit.currency }}</strong></td>
                  </tr>x
                </tbody>
                <tfoot class="table-secondary">
                  <tr>
                    <th>Общо</th>
                    <th>{{ planAmount.toFixed(2) }} {{ deposit.currency }}</th>
                    <th>{{ totalInterest.toFixed(2) }} {{ deposit.currency }}</th>
                    <th>{{ totalTax.toFixed(2) }} {{ deposit.currency }}</th>
                    <th>{{ totalNetInterest.toFixed(2) }} {{ deposit.currency }}</th>
                    <th><strong>{{ finalAmount.toFixed(2) }} {{ deposit.currency }}</strong></th>
                    <th>{{ averageEgl.toFixed(2) }}%</th>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="showPaymentPlan = false">Затвори</button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="showPaymentPlan" class="modal-backdrop fade show"></div>
  </div>
</template>

<script setup>
/**
 * Deposit Details View
 *
 * Displays comprehensive information about a specific deposit product including:
 * - Basic terms and conditions
 * - Features (capitalization, additional deposits, withdrawals, auto-renewal)
 * - Financial calculator with payment plan showing month-by-month breakdown
 * - Tax calculations (10% tax on interest)
 * - EGL (Effective Annual Rate) calculation
 */
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'

const route = useRoute()
const deposit = ref(null)
const loading = ref(true)
const showPaymentPlan = ref(false)
const planAmount = ref(0)

/**
 * Converts interest rate from percentage to decimal
 */
const rate = computed(() => deposit.value ? deposit.value.interestRate / 100 : 0)

/**
 * Tax rate applied to interest earnings (10% in Bulgaria)
 */
const TAX_RATE = 0.10

/**
 * Calculates a detailed payment plan showing monthly breakdown
 * Takes into account capitalization and calculates EGL (Effective Annual Rate)
 * @returns {Array} Array of payment plan entries for each month
 */
const paymentPlan = computed(() => {
  if (!deposit.value || planAmount.value <= 0) return []

  const plan = []
  const monthlyRate = rate.value / 12
  let currentAmount = planAmount.value
  let accumulatedInterest = 0

  for (let month = 1; month <= deposit.value.termMonths; month++) {
    const monthlyInterest = currentAmount * monthlyRate
    const tax = monthlyInterest * TAX_RATE
    const netInterest = monthlyInterest - tax
    accumulatedInterest += monthlyInterest

    let endAmount
    if (deposit.value.hasCapitalization) {
      // С капитализация - лихвата се добавя към главницата
      endAmount = currentAmount + monthlyInterest
    } else {
      // Без капитализация - лихвата се натрупва отделно
      endAmount = currentAmount
    }

    // Изчисляване на ЕГЛ за този месец
    const monthsElapsed = month
    const totalGain = accumulatedInterest
    const egl = (totalGain / planAmount.value) * (12 / monthsElapsed) * 100

    plan.push({
      month,
      startAmount: currentAmount,
      monthlyInterest,
      tax,
      netInterest,
      accumulatedInterest,
      endAmount,
      egl
    })

    // За следващия месец започваме с новата сума (ако има капитализация)
    if (deposit.value.hasCapitalization) {
      currentAmount = endAmount
    }
  }

  return plan
})

const totalInterest = computed(() => {
  if (paymentPlan.value.length === 0) return 0
  return paymentPlan.value[paymentPlan.value.length - 1].accumulatedInterest
})

const totalTax = computed(() => {
  if (paymentPlan.value.length === 0) return 0
  return paymentPlan.value.reduce((sum, payment) => sum + payment.tax, 0)
})

const totalNetInterest = computed(() => {
  if (paymentPlan.value.length === 0) return 0
  return paymentPlan.value.reduce((sum, payment) => sum + payment.netInterest, 0)
})

const averageEgl = computed(() => {
  if (paymentPlan.value.length === 0) return 0
  return paymentPlan.value[paymentPlan.value.length - 1].egl
})

const finalAmount = computed(() => {
  if (paymentPlan.value.length === 0) return planAmount.value
  return paymentPlan.value[paymentPlan.value.length - 1].endAmount
})

onMounted(async () => {
  try {
    deposit.value = await api.getDeposit(route.params.id)
    planAmount.value = deposit.value.minAmount
  } finally {
    loading.value = false
  }
})
</script>
