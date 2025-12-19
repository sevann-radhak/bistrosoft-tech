<template>
  <div class="create-order-view">
    <h1>Create Order</h1>
    <form @submit.prevent="handleSubmit" class="order-form">
      <ErrorMessage :message="orderStore.error || customerStore.error || productStore.error" />
      
      <div class="form-section">
        <label for="customerId" class="form-label">
          Customer <span class="required">*</span>
        </label>
        <input
          id="customerId"
          v-model="formData.customerId"
          type="text"
          required
          class="form-input"
          placeholder="Enter customer ID (Guid)"
        />
        <p class="hint">
          Enter the customer ID. <router-link to="/customers/create">Create a new customer</router-link> if needed.
        </p>
      </div>
      
      <div class="form-section">
        <label class="form-label">Products <span class="required">*</span></label>
        <ProductSelection
          v-model="formData.items"
          :loading="productStore.loading"
        />
        <p v-if="formData.items.length === 0" class="hint">
          Please select at least one product
        </p>
      </div>
      
      <div v-if="totalAmount > 0" class="total-section">
        <h3>Total: ${{ totalAmount.toFixed(2) }}</h3>
      </div>
      
      <div class="form-actions">
        <Button
          type="submit"
          :loading="orderStore.loading"
          :disabled="!isFormValid"
        >
          Create Order
        </Button>
        <Button
          type="button"
          variant="secondary"
          @click="$router.push('/')"
        >
          Cancel
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
import ProductSelection from '../components/forms/ProductSelection.vue'
import type { CreateOrderDto, CreateOrderItemDto } from '../services/api/types'

const router = useRouter()
const orderStore = useOrderStore()
const customerStore = useCustomerStore()
const productStore = useProductStore()

const formData = ref<CreateOrderDto>({
  customerId: '',
  items: []
})

onMounted(async () => {
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
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

h1 {
  margin-bottom: 2rem;
  color: #333;
}

.order-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 500;
  color: #333;
}

.required {
  color: #e74c3c;
}

.form-input {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-input:focus {
  outline: none;
  border-color: #3498db;
}

.hint {
  font-size: 0.875rem;
  color: #666;
  margin-top: 0.5rem;
}

.hint a {
  color: #3498db;
  text-decoration: none;
}

.hint a:hover {
  text-decoration: underline;
}

.total-section {
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 4px;
  text-align: right;
}

.total-section h3 {
  margin: 0;
  color: #27ae60;
  font-size: 1.5rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}
</style>

