<template>
  <div class="create-product-view">
    <div class="header-section">
      <h1>Add New Product</h1>
      <p class="subtitle">Add a new product to your restaurant menu</p>
    </div>
    
    <form @submit.prevent="handleSubmit" class="product-form">
      <ErrorMessage :message="productStore.error" />
      
      <div class="form-section">
        <InputField
          id="name"
          v-model="formData.name"
          label="Product Name"
          placeholder="e.g., Margherita Pizza"
          required
          :error="errors.name"
          @blur="validateName"
        />
      </div>
      
      <div class="form-section">
        <label for="price" class="form-label">
          Price <span class="required">*</span>
        </label>
        <div class="price-input-container">
          <span class="currency-symbol">$</span>
          <input
            id="price"
            v-model.number="formData.price"
            type="number"
            step="0.01"
            min="0"
            class="form-input price-input"
            placeholder="0.00"
            required
            :class="{ 'error': errors.price }"
            @blur="validatePrice"
          />
        </div>
        <p v-if="errors.price" class="error-message">{{ errors.price }}</p>
      </div>
      
      <div class="form-section">
        <label for="stockQuantity" class="form-label">
          Stock Quantity <span class="required">*</span>
        </label>
        <input
          id="stockQuantity"
          v-model.number="formData.stockQuantity"
          type="number"
          min="0"
          class="form-input"
          placeholder="Enter available quantity"
          required
          :class="{ 'error': errors.stockQuantity }"
          @blur="validateStockQuantity"
        />
        <p v-if="errors.stockQuantity" class="error-message">{{ errors.stockQuantity }}</p>
      </div>
      
      <div class="form-actions">
        <Button
          type="button"
          variant="secondary"
          size="large"
          @click="$router.push('/orders/create')"
        >
          Cancel
        </Button>
        <Button
          type="submit"
          size="large"
          :loading="productStore.loading"
          :disabled="!isFormValid"
        >
          Create Product
        </Button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useProductStore } from '../stores'
import { InputField, Button, ErrorMessage } from '../components/common'
import type { CreateProductDto } from '../services/api/types'

const router = useRouter()
const productStore = useProductStore()

const formData = ref<CreateProductDto>({
  name: '',
  price: 0,
  stockQuantity: 0
})

const errors = ref<Record<string, string>>({})

const isFormValid = computed(() => {
  return formData.value.name.trim() !== '' &&
         formData.value.price > 0 &&
         formData.value.stockQuantity >= 0 &&
         Object.keys(errors.value).length === 0
})

function validateName(): void {
  if (formData.value.name.trim() === '') {
    errors.value.name = 'Product name is required'
  } else {
    delete errors.value.name
  }
}

function validatePrice(): void {
  if (formData.value.price <= 0) {
    errors.value.price = 'Price must be greater than 0'
  } else {
    delete errors.value.price
  }
}

function validateStockQuantity(): void {
  if (formData.value.stockQuantity < 0) {
    errors.value.stockQuantity = 'Stock quantity cannot be negative'
  } else {
    delete errors.value.stockQuantity
  }
}

async function handleSubmit(): Promise<void> {
  productStore.clearError()
  validateName()
  validatePrice()
  validateStockQuantity()
  
  if (!isFormValid.value) {
    return
  }
  
  try {
    await productStore.createProduct(formData.value)
    await productStore.fetchProducts()
    router.push('/orders/create')
  } catch (error) {
    console.error('Failed to create product:', error)
  }
}
</script>

<style scoped>
.create-product-view {
  max-width: 700px;
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

.product-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 2.5rem;
  background: white;
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  color: #2c3e50;
  font-size: 1rem;
}

.required {
  color: #e74c3c;
}

.form-input {
  padding: 0.875rem 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1), 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
}

.form-input.error {
  border-color: #e74c3c;
}

.price-input-container {
  position: relative;
  display: flex;
  align-items: center;
}

.currency-symbol {
  position: absolute;
  left: 1rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #666;
  z-index: 1;
}

.price-input {
  padding-left: 2.5rem;
  width: 100%;
}

.error-message {
  color: #e74c3c;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  padding-top: 1rem;
  border-top: 2px solid #e0e0e0;
}

@media (max-width: 768px) {
  .create-product-view {
    padding: 1rem;
  }
  
  .header-section h1 {
    font-size: 2rem;
  }
  
  .product-form {
    padding: 1.5rem;
  }
  
  .form-actions {
    flex-direction: column-reverse;
  }
  
  .form-actions button {
    width: 100%;
  }
}
</style>
