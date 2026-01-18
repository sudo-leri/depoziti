import axios from 'axios'

/**
 * Axios instance configured for the Bank Products API
 */
const api = axios.create({
  baseURL: '/api'
})

/**
 * API service for communicating with the Bank Products backend
 */
export default {
  /**
   * Retrieves deposits with optional filtering and sorting
   * @param {Object} filters - Filter criteria
   * @param {number} [filters.bankId] - Filter by bank ID
   * @param {string} [filters.currency] - Filter by currency (BGN, EUR, USD)
   * @param {number} [filters.minTerm] - Minimum term in months
   * @param {number} [filters.maxTerm] - Maximum term in months
   * @param {number} [filters.minAmount] - Minimum amount available
   * @param {string} [filters.sortBy] - Field to sort by (term, minamount, bank, or default interest rate)
   * @returns {Promise<Array>} Array of deposit objects
   */
  async getDeposits(filters = {}) {
    const params = new URLSearchParams()
    if (filters.bankId) params.append('bankId', filters.bankId)
    if (filters.currency) params.append('currency', filters.currency)
    if (filters.minTerm) params.append('minTerm', filters.minTerm)
    if (filters.maxTerm) params.append('maxTerm', filters.maxTerm)
    if (filters.minAmount) params.append('minAmount', filters.minAmount)
    if (filters.sortBy) params.append('sortBy', filters.sortBy)

    const { data } = await api.get(`/deposits?${params}`)
    return data
  },

  /**
   * Retrieves a specific deposit by its ID
   * @param {number} id - The deposit ID
   * @returns {Promise<Object>} The deposit object with full details
   */
  async getDeposit(id) {
    const { data } = await api.get(`/deposits/${id}`)
    return data
  },

  /**
   * Retrieves all banks
   * @returns {Promise<Array>} Array of bank objects ordered alphabetically
   */
  async getBanks() {
    const { data } = await api.get('/banks')
    return data
  }
}
