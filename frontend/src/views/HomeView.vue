<template>
  <div class="home-view">
    <h1>Bistrosoft Store</h1>
    <p class="subtitle">Manage customers and orders</p>
    
    <div class="actions-grid">
      <div class="action-card">
        <h2>Create Customer</h2>
        <p>Register a new customer in the system</p>
        <Button @click="$router.push('/customers/create')">
          Create Customer
        </Button>
      </div>
      
      <div class="action-card">
        <h2>Create Order</h2>
        <p>Create a new order for a customer</p>
        <Button @click="$router.push('/orders/create')">
          Create Order
        </Button>
      </div>
    </div>
    
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCustomerStore, useProductStore } from '../stores'
import { Button } from '../components/common'

const router = useRouter()
const customerStore = useCustomerStore()
const productStore = useProductStore()

onMounted(async () => {
  if (customerStore.customers.length === 0) {
    try {
      await customerStore.fetchCustomer('')
    } catch (error) {
      console.error('Failed to fetch customers:', error)
    }
  }
})
</script>

<style scoped>
.home-view {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

h1 {
  text-align: center;
  margin-bottom: 0.5rem;
  color: #333;
}

.subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 3rem;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.action-card {
  padding: 2rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.action-card h2 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.action-card p {
  color: #666;
  margin-bottom: 1.5rem;
}

</style>

