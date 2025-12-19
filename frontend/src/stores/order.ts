import { defineStore } from 'pinia'
import { ref } from 'vue'
import { orderService } from '../services/api'
import type { CreateOrderDto, UpdateOrderStatusDto, OrderDto } from '../services/api/types'

export const useOrderStore = defineStore('order', () => {
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function createOrder(data: CreateOrderDto): Promise<OrderDto> {
    loading.value = true
    error.value = null
    try {
      const order = await orderService.create(data)
      return order
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create order'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateOrderStatus(id: string, status: string): Promise<OrderDto> {
    loading.value = true
    error.value = null
    try {
      const data: UpdateOrderStatusDto = {
        orderId: id,
        status: status as any
      }
      const order = await orderService.updateStatus(id, data)
      return order
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update order status'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError(): void {
    error.value = null
  }

  return {
    loading,
    error,
    createOrder,
    updateOrderStatus,
    clearError
  }
})



