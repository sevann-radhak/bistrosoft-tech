import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { productService } from '../services/api'
import type { ProductDto } from '../services/api/types'

export const useProductStore = defineStore('product', () => {
  const products = ref<ProductDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const productById = computed(() => (id: string) => {
    return products.value.find(p => p.id === id)
  })

  const availableProducts = computed(() => {
    return products.value.filter(p => p.stockQuantity > 0)
  })

  async function fetchProducts(): Promise<void> {
    loading.value = true
    error.value = null
    try {
      products.value = await productService.getAll()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch products'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError(): void {
    error.value = null
  }

  return {
    products,
    loading,
    error,
    productById,
    availableProducts,
    fetchProducts,
    clearError
  }
})

