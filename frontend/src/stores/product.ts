import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { productService } from '../services/api'
import type { ProductDto, CreateProductDto } from '../services/api/types'

interface ErrorInfo {
  message: string
  title?: string
  fieldErrors?: Record<string, string[]>
}

export const useProductStore = defineStore('product', () => {
  const products = ref<ProductDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const errorInfo = ref<ErrorInfo | null>(null)

  const productById = computed(() => (id: string) => {
    return products.value.find(p => p.id === id)
  })

  const availableProducts = computed(() => {
    return products.value.filter(p => p.stockQuantity > 0)
  })

  async function fetchProducts(): Promise<void> {
    loading.value = true
    error.value = null
    errorInfo.value = null
    try {
      products.value = await productService.getAll()
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to fetch products'
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

  async function createProduct(data: CreateProductDto): Promise<ProductDto> {
    loading.value = true
    error.value = null
    errorInfo.value = null
    try {
      const product = await productService.create(data)
      products.value.push(product)
      return product
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to create product'
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
    products,
    loading,
    error,
    errorInfo,
    productById,
    availableProducts,
    fetchProducts,
    createProduct,
    clearError
  }
})



