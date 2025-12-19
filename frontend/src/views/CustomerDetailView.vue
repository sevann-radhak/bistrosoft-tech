<template>
  <div class="customer-detail-view">
    <div v-if="loading" class="loading">
      <p>Loading customer details...</p>
    </div>
    
    <div v-else-if="error" class="error">
      <ErrorMessage :message="error" />
      <Button @click="loadCustomer">Retry</Button>
      <Button variant="secondary" @click="$router.push('/customers')">
        Back to Customers
      </Button>
    </div>
    
    <div v-else-if="customer">
      <div class="header-section">
        <Button
          variant="secondary"
          size="small"
          @click="$router.push('/customers')"
        >
          ← Back to Customers
        </Button>
        <h1>{{ customer.name }}</h1>
        <p class="subtitle">Customer Details</p>
      </div>
      
      <div class="customer-info-card">
        <div class="info-row">
          <span class="info-label">Email:</span>
          <span class="info-value">{{ customer.email }}</span>
        </div>
        <div v-if="customer.phoneNumber" class="info-row">
          <span class="info-label">Phone:</span>
          <span class="info-value">{{ customer.phoneNumber }}</span>
        </div>
        <div class="info-row">
          <span class="info-label">Total Orders:</span>
          <span class="info-value">{{ customer.orders?.length || 0 }}</span>
        </div>
      </div>
      
      <div class="actions-section">
        <Button
          size="large"
          @click="$router.push(`/customers/${customer.id}/orders`)"
        >
          View All Orders
        </Button>
        <Button
          variant="secondary"
          size="large"
          @click="$router.push('/orders/create')"
        >
          Create New Order
        </Button>
      </div>
      
      <div v-if="customer.orders && customer.orders.length > 0" class="orders-preview">
        <h2>Recent Orders</h2>
        <div class="orders-list">
          <div
            v-for="order in customer.orders.slice(0, 3)"
            :key="order.id"
            class="order-preview-card"
            @click="$router.push(`/customers/${customer.id}/orders`)"
          >
            <div class="order-preview-header">
              <span class="order-id">Order #{{ order.id.substring(0, 8) }}</span>
              <OrderStatusDisplay :status="order.status" />
            </div>
            <div class="order-preview-meta">
              <span class="order-date">{{ formatDate(order.createdAt) }}</span>
              <span class="order-total">${{ order.totalAmount.toFixed(2) }}</span>
            </div>
          </div>
        </div>
        <div v-if="customer.orders.length > 3" class="view-all-link">
          <Button variant="secondary" @click="$router.push(`/customers/${customer.id}/orders`)">
            View All {{ customer.orders.length }} Orders
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCustomerStore } from '../stores'
import { Button, ErrorMessage } from '../components/common'
import OrderStatusDisplay from '../components/forms/OrderStatusDisplay.vue'
import type { CustomerDto } from '../services/api/types'

const route = useRoute()
const router = useRouter()
const customerStore = useCustomerStore()

const customer = ref<CustomerDto | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const customerId = computed(() => route.params.id as string)

onMounted(async () => {
  await loadCustomer()
})

watch(() => route.params.id, async () => {
  await loadCustomer()
})

async function loadCustomer(): Promise<void> {
  loading.value = true
  error.value = null
  
  try {
    await customerStore.fetchCustomer(customerId.value)
    customer.value = customerStore.currentCustomer
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load customer'
    console.error('Failed to load customer:', err)
  } finally {
    loading.value = false
  }
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.customer-detail-view {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.error {
  text-align: center;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}

.header-section {
  margin-bottom: 2rem;
}

.header-section h1 {
  margin: 1rem 0 0.5rem 0;
  font-size: 2.5rem;
  font-weight: 700;
  color: #2c3e50;
}

.subtitle {
  color: #7f8c8d;
  font-size: 1.125rem;
  margin: 0;
}

.customer-info-card {
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 20px;
  padding: 2.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  position: relative;
  overflow: hidden;
}

.customer-info-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 4px;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 1rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-row:last-child {
  border-bottom: none;
}

.info-label {
  font-weight: 600;
  color: #666;
}

.info-value {
  color: #2c3e50;
  font-size: 1.125rem;
}

.actions-section {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  justify-content: center;
  flex-wrap: wrap;
}

.actions-section .btn {
  min-width: 180px;
}

.orders-preview {
  margin-top: 2rem;
}

.orders-preview h2 {
  margin-bottom: 1.5rem;
  color: #2c3e50;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.order-preview-card {
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.order-preview-card:hover {
  border-color: #667eea;
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.2);
  transform: translateX(8px);
}

.order-preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.order-id {
  font-weight: 600;
  color: #2c3e50;
}

.order-preview-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.order-date {
  color: #666;
  font-size: 0.875rem;
}

.order-total {
  font-weight: 700;
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-size: 1.25rem;
}

.view-all-link {
  text-align: center;
}
</style>
