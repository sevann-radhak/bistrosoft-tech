<template>
  <div class="create-customer-view">
    <h1>Create Customer</h1>
    <form @submit.prevent="handleSubmit" class="customer-form">
      <ErrorMessage :message="customerStore.error" />
      
      <InputField
        id="name"
        v-model="formData.name"
        label="Name"
        placeholder="Enter customer name"
        required
        :error="errors.name"
        @blur="validateName"
      />
      
      <InputField
        id="email"
        v-model="formData.email"
        type="email"
        label="Email"
        placeholder="Enter email address"
        required
        :error="errors.email"
        @blur="validateEmail"
      />
      
      <InputField
        id="phoneNumber"
        v-model="formData.phoneNumber"
        type="tel"
        label="Phone Number"
        placeholder="Enter phone number (optional)"
        :error="errors.phoneNumber"
      />
      
      <div class="form-actions">
        <Button
          type="submit"
          :loading="customerStore.loading"
          :disabled="!isFormValid"
        >
          Create Customer
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
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCustomerStore } from '../stores'
import { InputField, Button, ErrorMessage } from '../components/common'
import type { CreateCustomerDto } from '../services/api/types'

const router = useRouter()
const customerStore = useCustomerStore()

const formData = ref<CreateCustomerDto>({
  name: '',
  email: '',
  phoneNumber: ''
})

const errors = ref<Record<string, string>>({})

const isFormValid = computed(() => {
  return formData.value.name.trim() !== '' &&
         formData.value.email.trim() !== '' &&
         isValidEmail(formData.value.email) &&
         Object.keys(errors.value).length === 0
})

function validateName(): void {
  if (formData.value.name.trim() === '') {
    errors.value.name = 'Name is required'
  } else {
    delete errors.value.name
  }
}

function validateEmail(): void {
  if (formData.value.email.trim() === '') {
    errors.value.email = 'Email is required'
  } else if (!isValidEmail(formData.value.email)) {
    errors.value.email = 'Please enter a valid email address'
  } else {
    delete errors.value.email
  }
}

function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

async function handleSubmit(): Promise<void> {
  customerStore.clearError()
  validateName()
  validateEmail()
  
  if (!isFormValid.value) {
    return
  }
  
  try {
    const customer = await customerStore.createCustomer(formData.value)
    router.push(`/customers/${customer.id}/orders`)
  } catch (error) {
    console.error('Failed to create customer:', error)
  }
}
</script>

<style scoped>
.create-customer-view {
  max-width: 700px;
  margin: 0 auto;
  padding: 2rem;
}

h1 {
  margin-bottom: 0.5rem;
  font-size: 2.5rem;
  font-weight: 700;
  color: #2c3e50;
  text-align: center;
}

.customer-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 2.5rem;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin-top: 2rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
  padding-top: 1.5rem;
  border-top: 2px solid #f0f0f0;
}
</style>



