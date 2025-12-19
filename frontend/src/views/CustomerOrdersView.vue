<template>
  <div class="customer-orders-view">
    <div v-if="customerStore.loading" class="loading">
      Loading customer orders...
    </div>
    
    <div v-else-if="customerStore.error" class="error">
      <ErrorMessage :message="customerStore.error" />
      <Button @click="loadOrders">Retry</Button>
    </div>
    
    <div v-else>
      <div class="header">
        <h1>Customer Orders</h1>
        <div v-if="customerStore.currentCustomer" class="customer-info">
          <h2>{{ customerStore.currentCustomer.name }}</h2>
          <p>{{ customerStore.currentCustomer.email }}</p>
          <p v-if="customerStore.currentCustomer.phoneNumber">
            {{ customerStore.currentCustomer.phoneNumber }}
          </p>
        </div>
      </div>
      
      <div v-if="orders.length === 0" class="empty-state">
        <p>No orders found for this customer.</p>
        <Button @click="$router.push('/orders/create')">
          Create First Order
        </Button>
      </div>
      
      <div v-else class="orders-list">
        <div
          v-for="order in orders"
          :key="order.id"
          class="order-card"
        >
          <div class="order-header">
            <div>
              <h3>Order #{{ order.id.substring(0, 8) }}</h3>
              <p class="order-date">
                Created: {{ formatDate(order.createdAt) }}
              </p>
            </div>
            <div class="order-meta">
              <OrderStatusDisplay :status="order.status" />
              <p class="order-total">${{ order.totalAmount.toFixed(2) }}</p>
            </div>
          </div>
          
          <div class="order-items">
            <h4>Items:</h4>
            <ul>
              <li
                v-for="item in order.orderItems"
                :key="item.id"
                class="order-item"
              >
                <span class="item-name">
                  {{ item.product?.name || 'Product' }}
                </span>
                <span class="item-details">
                  {{ item.quantity }} × ${{ item.unitPrice.toFixed(2) }} = 
                  ${{ (item.quantity * item.unitPrice).toFixed(2) }}
                </span>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCustomerStore } from '../stores'
import { Button, ErrorMessage } from '../components/common'
import OrderStatusDisplay from '../components/forms/OrderStatusDisplay.vue'
import type { OrderDto } from '../services/api/types'

const route = useRoute()
const router = useRouter()
const customerStore = useCustomerStore()

const customerId = computed(() => route.params.id as string)
const orders = computed(() => customerStore.currentCustomer?.orders || [])

onMounted(async () => {
  await loadOrders()
})

async function loadOrders(): Promise<void> {
  try {
    await customerStore.fetchCustomer(customerId.value)
    await customerStore.fetchCustomerOrders(customerId.value)
  } catch (error) {
    console.error('Failed to load orders:', error)
  }
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.customer-orders-view {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem;
}

.loading,
.error {
  text-align: center;
  padding: 2rem;
}

.header {
  margin-bottom: 2rem;
}

.header h1 {
  margin-bottom: 1rem;
  color: #333;
}

.customer-info {
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 4px;
  margin-bottom: 2rem;
}

.customer-info h2 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.customer-info p {
  margin: 0.25rem 0;
  color: #666;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.empty-state p {
  margin-bottom: 1rem;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.order-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 1.5rem;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #eee;
}

.order-header h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.order-date {
  font-size: 0.875rem;
  color: #666;
  margin: 0;
}

.order-meta {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
}

.order-total {
  font-size: 1.25rem;
  font-weight: 600;
  color: #27ae60;
  margin: 0;
}

.order-items {
  margin-top: 1rem;
}

.order-items h4 {
  margin: 0 0 0.75rem 0;
  color: #333;
  font-size: 1rem;
}

.order-items ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.order-item {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.order-item:last-child {
  border-bottom: none;
}

.item-name {
  font-weight: 500;
  color: #333;
}

.item-details {
  color: #666;
}
</style>



