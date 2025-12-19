import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { customerService } from '../services/api'
import type { CustomerDto, CreateCustomerDto, OrderDto } from '../services/api/types'

export const useCustomerStore = defineStore('customer', () => {
  const customers = ref<CustomerDto[]>([])
  const currentCustomer = ref<CustomerDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const customerById = computed(() => (id: string) => {
    return customers.value.find(c => c.id === id)
  })

  async function createCustomer(data: CreateCustomerDto): Promise<CustomerDto> {
    loading.value = true
    error.value = null
    try {
      const customer = await customerService.create(data)
      customers.value.push(customer)
      return customer
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomer(id: string): Promise<void> {
    loading.value = true
    error.value = null
    try {
      const customer = await customerService.getById(id)
      currentCustomer.value = customer
      const index = customers.value.findIndex(c => c.id === id)
      if (index !== -1) {
        customers.value[index] = customer
      } else {
        customers.value.push(customer)
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomerOrders(id: string): Promise<OrderDto[]> {
    loading.value = true
    error.value = null
    try {
      const orders = await customerService.getOrders(id)
      if (currentCustomer.value && currentCustomer.value.id === id) {
        currentCustomer.value.orders = orders
      }
      return orders
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch customer orders'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError(): void {
    error.value = null
  }

  function setCurrentCustomer(customer: CustomerDto | null): void {
    currentCustomer.value = customer
  }

  return {
    customers,
    currentCustomer,
    loading,
    error,
    customerById,
    createCustomer,
    fetchCustomer,
    fetchCustomerOrders,
    clearError,
    setCurrentCustomer
  }
})
