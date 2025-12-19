import apiClient from './client'
import type { OrderDto, CreateOrderDto, UpdateOrderStatusDto } from './types'

export const orderService = {
  create: (data: CreateOrderDto): Promise<OrderDto> => 
    apiClient.post('/orders', data),
  
  updateStatus: (id: string, data: UpdateOrderStatusDto): Promise<OrderDto> => 
    apiClient.put(`/orders/${id}/status`, data)
}

