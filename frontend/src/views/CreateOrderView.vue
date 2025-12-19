<template>
  <div class="create-order-view">
    <div class="header-section">
      <h1>New Order</h1>
      <p class="subtitle">Quick order creation for your restaurant</p>
    </div>
    
    <form @submit.prevent="handleSubmit" class="order-form">
      <ErrorMessage 
        :message="orderStore.error || customerStore.error || productStore.error"
        :title="orderStore.errorInfo?.title || customerStore.errorInfo?.title || productStore.errorInfo?.title"
        :field-errors="orderStore.errorInfo?.fieldErrors || customerStore.errorInfo?.fieldErrors || productStore.errorInfo?.fieldErrors"
      />
      
      <div class="form-section customer-section">
        <label class="section-label">
          <span class="label-icon">👤</span>
          Select Customer
        </label>
        <CustomerSelector
          v-model="formData.customerId"
        />
      </div>
      
      <div class="form-section products-section">
        <label class="section-label">
          <span class="label-icon">🍽️</span>
          Select Products
        </label>
        <ProductSelection
          v-model="formData.items"
          :loading="productStore.loading"
        />
      </div>
      
      <div v-if="totalAmount > 0" class="total-section">
        <div class="total-content">
          <span class="total-label">Total Amount</span>
          <span class="total-value">${{ totalAmount.toFixed(2) }}</span>
        </div>
        <div class="items-count">{{ totalItems }} item{{ totalItems !== 1 ? 's' : '' }}</div>
      </div>
      
      <div class="form-actions">
        <Button
          type="button"
          variant="secondary"
          size="large"
          @click="$router.push('/')"
        >
          Cancel
        </Button>
        <Button
          type="submit"
          size="large"
          :loading="orderStore.loading"
          :disabled="!isFormValid"
        >
          Create Order
        </Button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useOrderStore, useCustomerStore, useProductStore } from '../stores'
import { Button, ErrorMessage } from '../components/common'
import CustomerSelector from '../components/forms/CustomerSelector.vue'
import ProductSelection from '../components/forms/ProductSelection.vue'
import type { CreateOrderDto } from '../services/api/types'

const router = useRouter()
const orderStore = useOrderStore()
const customerStore = useCustomerStore()
const productStore = useProductStore()

const formData = ref<CreateOrderDto>({
  customerId: '',
  items: []
})

onMounted(async () => {
  if (customerStore.customers.length === 0) {
    await customerStore.fetchAllCustomers()
  }
  if (productStore.products.length === 0) {
    await productStore.fetchProducts()
  }
})

const totalAmount = computed(() => {
  return formData.value.items.reduce((total, item) => {
    const product = productStore.productById(item.productId)
    if (product) {
      return total + (product.price * item.quantity)
    }
    return total
  }, 0)
})

const totalItems = computed(() => {
  return formData.value.items.reduce((sum, item) => sum + item.quantity, 0)
})

const isFormValid = computed(() => {
  return formData.value.customerId !== '' &&
         formData.value.items.length > 0 &&
         formData.value.items.every(item => item.quantity > 0)
})

async function handleSubmit(): Promise<void> {
  orderStore.clearError()
  customerStore.clearError()
  productStore.clearError()
  
  if (!isFormValid.value) {
    return
  }
  
  try {
    const order = await orderStore.createOrder(formData.value)
    router.push(`/customers/${formData.value.customerId}/orders`)
  } catch (error) {
    console.error('Failed to create order:', error)
  }
}
</script>

<style scoped>
.create-order-view {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.header-section {
  text-align: center;
  margin-bottom: 3rem;
}

.header-section h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2.5rem;
  font-weight: 700;
  color: #2c3e50;
}

.subtitle {
  margin: 0;
  font-size: 1.125rem;
  color: #7f8c8d;
}

.order-form {
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.section-label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.label-icon {
  font-size: 1.5rem;
}

.customer-section,
.products-section {
  padding: 2.5rem;
  background: white;
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.customer-section:hover,
.products-section:hover {
  box-shadow: 0 12px 40px rgba(102, 126, 234, 0.15);
  transform: translateY(-2px);
}

.total-section {
  padding: 2.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 20px;
  color: white;
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.4);
  position: relative;
  overflow: hidden;
}

.total-section::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 0%, transparent 70%);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0%, 100% {
    transform: rotate(0deg);
  }
  50% {
    transform: rotate(180deg);
  }
}

.total-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.total-label {
  font-size: 1.125rem;
  font-weight: 500;
  opacity: 0.9;
}

.total-value {
  font-size: 3rem;
  font-weight: 800;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
  letter-spacing: -1px;
}

.items-count {
  font-size: 0.875rem;
  opacity: 0.8;
  text-align: right;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  padding-top: 1.5rem;
  border-top: 2px solid #e0e0e0;
  margin-top: 1rem;
}

.form-actions .btn {
  min-width: 150px;
}

@media (max-width: 768px) {
  .create-order-view {
    padding: 1rem;
  }
  
  .header-section h1 {
    font-size: 2rem;
  }
  
  .customer-section,
  .products-section {
    padding: 1.5rem;
  }
  
  .total-value {
    font-size: 2rem;
  }
  
  .form-actions {
    flex-direction: column-reverse;
  }
  
  .form-actions button {
    width: 100%;
  }
}
</style>



