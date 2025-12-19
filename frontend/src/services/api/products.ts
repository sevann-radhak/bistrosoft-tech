import apiClient from './client'
import type { ProductDto } from './types'

export const productService = {
  getAll: (): Promise<ProductDto[]> => 
    apiClient.get('/products')
}

