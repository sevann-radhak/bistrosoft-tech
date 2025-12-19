<template>
  <div class="product-selection">
    <div v-if="loading" class="loading-state">
      <p>Loading products...</p>
    </div>
    
    <div v-else-if="products.length === 0" class="empty-state">
      <p>No products available</p>
      <Button
        type="button"
        variant="secondary"
        size="small"
        @click="handleAddProduct"
      >
        + Add Product
      </Button>
    </div>
    
    <div v-else class="products-grid">
      <div
        v-for="product in availableProducts"
        :key="product.id"
        class="product-card"
        :class="{ 'selected': isSelected(product.id) }"
      >
        <div class="product-header">
          <h3 class="product-name">{{ product.name }}</h3>
          <div class="product-price">${{ product.price.toFixed(2) }}</div>
        </div>
        
        <div class="product-stock">
          <span :class="{ 'low-stock': product.stockQuantity < 10, 'out-of-stock': product.stockQuantity === 0 }">
            {{ product.stockQuantity === 0 ? 'Out of stock' : `${product.stockQuantity} available` }}
          </span>
        </div>
        
        <div class="product-controls">
          <button
            type="button"
            class="quantity-btn"
            :disabled="getQuantity(product.id) === 0"
            @click="decreaseQuantity(product.id)"
          >
            −
          </button>
          <input
            :id="`qty-${product.id}`"
            :value="getQuantity(product.id)"
            type="number"
            min="0"
            :max="product.stockQuantity"
            class="quantity-input"
            :disabled="product.stockQuantity === 0"
            @input="updateQuantity(product.id, $event)"
          />
          <button
            type="button"
            class="quantity-btn"
            :disabled="product.stockQuantity === 0 || getQuantity(product.id) >= product.stockQuantity"
            @click="increaseQuantity(product.id)"
          >
            +
          </button>
        </div>
      </div>
    </div>
    
    <div v-if="selectedItems.length > 0" class="selected-summary">
      <h4>Selected Items ({{ selectedItems.length }})</h4>
      <div class="selected-list">
        <div
          v-for="item in selectedItems"
          :key="item.productId"
          class="selected-item"
        >
          <span class="item-name">{{ getProductName(item.productId) }}</span>
          <span class="item-quantity">×{{ item.quantity }}</span>
          <span class="item-total">${{ getItemTotal(item).toFixed(2) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useProductStore } from '../../stores'
import { Button } from '../common'
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

const router = useRouter()
const productStore = useProductStore()
const quantities = ref<Record<string, number>>({})

const products = computed(() => productStore.products)
const loading = computed(() => productStore.loading)
const availableProducts = computed(() => products.value.filter(p => p.stockQuantity > 0))

const selectedItems = computed(() => {
  return Object.entries(quantities.value)
    .filter(([_, qty]) => qty > 0)
    .map(([pid, qty]) => ({
      productId: pid,
      quantity: qty
    }))
})

onMounted(async () => {
  if (products.value.length === 0) {
    await productStore.fetchProducts()
  }
  initializeQuantities()
})

watch(() => props.modelValue, initializeQuantities, { deep: true })

watch(selectedItems, (newItems) => {
  emit('update:modelValue', newItems)
}, { deep: true })

function initializeQuantities(): void {
  const newQuantities: Record<string, number> = {}
  props.modelValue.forEach(item => {
    newQuantities[item.productId] = item.quantity
  })
  quantities.value = newQuantities
}

function getQuantity(productId: string): number {
  return quantities.value[productId] || 0
}

function isSelected(productId: string): boolean {
  return getQuantity(productId) > 0
}

function increaseQuantity(productId: string): void {
  const current = getQuantity(productId)
  const product = productStore.productById(productId)
  if (product && current < product.stockQuantity) {
    quantities.value[productId] = current + 1
  }
}

function decreaseQuantity(productId: string): void {
  const current = getQuantity(productId)
  if (current > 0) {
    quantities.value[productId] = current - 1
  }
}

function updateQuantity(productId: string, event: Event): void {
  const input = event.target as HTMLInputElement
  const quantity = Math.max(0, Math.min(parseInt(input.value) || 0, productStore.productById(productId)?.stockQuantity || 0))
  quantities.value[productId] = quantity
}

function getProductName(productId: string): string {
  return productStore.productById(productId)?.name || 'Unknown'
}

function getItemTotal(item: SelectedItem): number {
  const product = productStore.productById(item.productId)
  return product ? product.price * item.quantity : 0
}

function handleAddProduct(): void {
  router.push('/products/create')
}
</script>

<style scoped>
.product-selection {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.loading-state,
.empty-state {
  padding: 3rem;
  text-align: center;
  color: #666;
}

.empty-state {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.product-card {
  padding: 1.25rem;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  background: white;
  transition: all 0.2s;
  cursor: pointer;
}

.product-card:hover {
  border-color: #3498db;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.product-card.selected {
  border-color: #4caf50;
  background-color: #f1f8f4;
}

.product-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.75rem;
}

.product-name {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #333;
  flex: 1;
}

.product-price {
  font-size: 1.25rem;
  font-weight: 700;
  color: #27ae60;
  white-space: nowrap;
}

.product-stock {
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.product-stock span {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  background-color: #e8f5e9;
  color: #2e7d32;
}

.product-stock .low-stock {
  background-color: #fff3e0;
  color: #e65100;
}

.product-stock .out-of-stock {
  background-color: #ffebee;
  color: #c62828;
}

.product-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  justify-content: center;
}

.quantity-btn {
  width: 40px;
  height: 40px;
  border: 2px solid #ddd;
  background: white;
  border-radius: 8px;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
}

.quantity-btn:hover:not(:disabled) {
  background-color: #3498db;
  border-color: #3498db;
  color: white;
}

.quantity-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.quantity-input {
  width: 60px;
  height: 40px;
  padding: 0.5rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  text-align: center;
  font-size: 1.125rem;
  font-weight: 600;
}

.quantity-input:focus {
  outline: none;
  border-color: #3498db;
}

.quantity-input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.selected-summary {
  margin-top: 1rem;
  padding: 1.5rem;
  background-color: #f8f9fa;
  border-radius: 12px;
  border: 2px solid #e0e0e0;
}

.selected-summary h4 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.125rem;
}

.selected-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.selected-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: white;
  border-radius: 8px;
}

.item-name {
  font-weight: 500;
  color: #333;
  flex: 1;
}

.item-quantity {
  color: #666;
  margin: 0 1rem;
}

.item-total {
  font-weight: 600;
  color: #27ae60;
  font-size: 1.125rem;
}
</style>
