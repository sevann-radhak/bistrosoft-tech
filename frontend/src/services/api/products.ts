import apiClient from './client'
import type { ProductDto, CreateProductDto } from './types'

export const productService = {
  getAll: (): Promise<ProductDto[]> => 
    apiClient.get('/products'),
  create: (data: CreateProductDto): Promise<ProductDto> =>
    apiClient.post('/products', data)
}



