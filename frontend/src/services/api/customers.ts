import apiClient from './client'
import type { CustomerDto, CreateCustomerDto, OrderDto } from './types'

export const customerService = {
  create: (data: CreateCustomerDto): Promise<CustomerDto> => 
    apiClient.post('/customers', data),
  
  getById: (id: string): Promise<CustomerDto> => 
    apiClient.get(`/customers/${id}`),
  
  getOrders: (id: string): Promise<OrderDto[]> => 
    apiClient.get(`/customers/${id}/orders`)
}

