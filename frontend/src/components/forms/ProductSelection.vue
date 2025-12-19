<template>
  <div class="product-selection">
    <div v-if="products.length === 0" class="empty-state">
      <p v-if="loading">Loading products...</p>
      <p v-else>No products available</p>
    </div>
    <div v-else class="product-list">
      <div
        v-for="product in products"
        :key="product.id"
        class="product-item"
        :class="{ 'out-of-stock': product.stockQuantity === 0 }"
      >
        <div class="product-info">
          <h4>{{ product.name }}</h4>
          <p class="product-price">${{ product.price.toFixed(2) }}</p>
          <p class="product-stock">Stock: {{ product.stockQuantity }}</p>
        </div>
        <div class="product-actions">
          <input
            :id="`quantity-${product.id}`"
            v-model.number="quantities[product.id]"
            type="number"
            min="0"
            :max="product.stockQuantity"
            class="quantity-input"
            :disabled="product.stockQuantity === 0"
            @input="updateQuantity(product.id, $event)"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useProductStore } from '../../stores'
import type { ProductDto } from '../../services/api/types'

interface SelectedItem {
  productId: string
  quantity: number
}

interface Props {
  modelValue: SelectedItem[]
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: SelectedItem[]]
}>()

const productStore = useProductStore()
const quantities = ref<Record<string, number>>({})

const products = computed(() => productStore.products)
const loading = computed(() => productStore.loading)

onMounted(async () => {
  if (products.value.length === 0) {
    await productStore.fetchProducts()
  }
  initializeQuantities()
})

watch(() => props.modelValue, initializeQuantities, { deep: true })

function initializeQuantities(): void {
  const newQuantities: Record<string, number> = {}
  props.modelValue.forEach(item => {
    newQuantities[item.productId] = item.quantity
  })
  quantities.value = newQuantities
}

function updateQuantity(productId: string, event: Event): void {
  const input = event.target as HTMLInputElement
  const quantity = parseInt(input.value) || 0
  quantities.value[productId] = quantity
  
  const selectedItems: SelectedItem[] = Object.entries(quantities.value)
    .filter(([_, qty]) => qty > 0)
    .map(([pid, qty]) => ({
      productId: pid,
      quantity: qty
    }))
  
  emit('update:modelValue', selectedItems)
}
</script>

<style scoped>
.product-selection {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.empty-state {
  padding: 2rem;
  text-align: center;
  color: #666;
}

.product-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.product-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  transition: all 0.2s;
}

.product-item:hover {
  border-color: #3498db;
}

.product-item.out-of-stock {
  opacity: 0.6;
  background-color: #f5f5f5;
}

.product-info {
  flex: 1;
}

.product-info h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1.125rem;
}

.product-price {
  font-weight: 600;
  color: #27ae60;
  margin: 0.25rem 0;
}

.product-stock {
  font-size: 0.875rem;
  color: #666;
  margin: 0.25rem 0;
}

.quantity-input {
  width: 80px;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  text-align: center;
}

.quantity-input:focus {
  outline: none;
  border-color: #3498db;
}
</style>
