import { defineStore } from 'pinia'
import { ref } from 'vue'
import { orderService } from '../services/api'
import type { CreateOrderDto, UpdateOrderStatusDto, OrderDto } from '../services/api/types'

interface ErrorInfo {
  message: string
  title?: string
  fieldErrors?: Record<string, string[]>
}

export const useOrderStore = defineStore('order', () => {
  const loading = ref(false)
  const error = ref<string | null>(null)
  const errorInfo = ref<ErrorInfo | null>(null)

  async function createOrder(data: CreateOrderDto): Promise<OrderDto> {
    loading.value = true
    error.value = null
    errorInfo.value = null
    try {
      const order = await orderService.create(data)
      return order
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to create order'
      error.value = errorMessage
      errorInfo.value = {
        message: errorMessage,
        title: (err as any)?.title,
        fieldErrors: (err as any)?.errors
      }
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateOrderStatus(id: string, status: string): Promise<OrderDto> {
    loading.value = true
    error.value = null
    errorInfo.value = null
    try {
      const data: UpdateOrderStatusDto = {
        orderId: id,
        status: status as any
      }
      const order = await orderService.updateStatus(id, data)
      return order
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to update order status'
      error.value = errorMessage
      errorInfo.value = {
        message: errorMessage,
        title: (err as any)?.title,
        fieldErrors: (err as any)?.errors
      }
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError(): void {
    error.value = null
    errorInfo.value = null
  }

  return {
    loading,
    error,
    errorInfo,
    createOrder,
    updateOrderStatus,
    clearError
  }
})



