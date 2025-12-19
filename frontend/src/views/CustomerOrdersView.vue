<template>
  <div class="customer-orders-view">
    <div v-if="loading" class="loading">
      <p>Loading customer orders...</p>
    </div>
    
    <div v-else-if="error" class="error">
      <ErrorMessage :message="error" />
      <Button @click="loadOrders">Retry</Button>
    </div>
    
    <div v-else>
      <div class="header">
        <h1>Customer Orders</h1>
        <div v-if="customer" class="customer-info">
          <h2>{{ customer.name }}</h2>
          <p>{{ customer.email }}</p>
          <p v-if="customer.phoneNumber">
            {{ customer.phoneNumber }}
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
          
          <div class="order-actions">
            <h4>Order Actions:</h4>
            <div class="action-buttons">
              <Button
                v-if="canMarkAsPaid(order.status)"
                size="large"
                @click="updateStatus(order.id, 'Paid')"
                :loading="updatingStatusId === order.id && orderStore.loading"
              >
                ✓ Mark as Paid
              </Button>
              <Button
                v-if="canMarkAsShipped(order.status)"
                size="large"
                @click="updateStatus(order.id, 'Shipped')"
                :loading="updatingStatusId === order.id && orderStore.loading"
              >
                🚚 Mark as Shipped
              </Button>
              <Button
                v-if="canMarkAsDelivered(order.status)"
                size="large"
                @click="updateStatus(order.id, 'Delivered')"
                :loading="updatingStatusId === order.id && orderStore.loading"
              >
                ✅ Mark as Delivered
              </Button>
              <Button
                v-if="canCancel(order.status)"
                variant="danger"
                size="large"
                @click="updateStatus(order.id, 'Cancelled')"
                :loading="updatingStatusId === order.id && orderStore.loading"
              >
                ✕ Cancel Order
              </Button>
              <p v-if="isFinalStatus(order.status)" class="final-status-message">
                This order has been completed and cannot be modified.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCustomerStore, useOrderStore } from '../stores'
import { Button, ErrorMessage } from '../components/common'
import OrderStatusDisplay from '../components/forms/OrderStatusDisplay.vue'
import type { OrderDto, CustomerDto, OrderStatus } from '../services/api/types'

const route = useRoute()
const router = useRouter()
const customerStore = useCustomerStore()
const orderStore = useOrderStore()

const customerId = computed(() => route.params.id as string)
const customer = ref<CustomerDto | null>(null)
const orders = ref<OrderDto[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const updatingStatusId = ref<string | null>(null)

onMounted(async () => {
  await loadOrders()
})

watch(() => route.params.id, async () => {
  await loadOrders()
})

async function loadOrders(): Promise<void> {
  loading.value = true
  error.value = null
  
  try {
    await customerStore.fetchCustomer(customerId.value)
    const fetchedOrders = await customerStore.fetchCustomerOrders(customerId.value)
    
    customer.value = customerStore.currentCustomer
    orders.value = fetchedOrders
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load orders'
    console.error('Failed to load orders:', err)
  } finally {
    loading.value = false
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

function canMarkAsPaid(status: OrderStatus | string): boolean {
  return status === 'Pending' || status === 0
}

function canMarkAsShipped(status: OrderStatus | string): boolean {
  return status === 'Paid' || status === 1
}

function canMarkAsDelivered(status: OrderStatus | string): boolean {
  return status === 'Shipped' || status === 2
}

function canCancel(status: OrderStatus | string): boolean {
  return status === 'Pending' || status === 'Paid' || status === 0 || status === 1
}

function isFinalStatus(status: OrderStatus | string): boolean {
  return status === 'Delivered' || status === 'Cancelled' || status === 3 || status === 4
}

async function updateStatus(orderId: string, newStatus: string): Promise<void> {
  updatingStatusId.value = orderId
  orderStore.clearError()
  
  try {
    await orderStore.updateOrderStatus(orderId, newStatus)
    await loadOrders()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to update order status'
    console.error('Failed to update order status:', err)
  } finally {
    updatingStatusId.value = null
  }
}
</script>

<style scoped>
.customer-orders-view {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.loading p {
  font-size: 1.125rem;
}

.error {
  text-align: center;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
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
  border: 2px solid #e0e0e0;
  border-radius: 16px;
  padding: 2rem;
  background: white;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  position: relative;
  overflow: visible;
}

.order-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 4px;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  transform: scaleX(0);
  transition: transform 0.3s ease;
}

.order-card:hover {
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.15);
  transform: translateY(-2px);
  border-color: #667eea;
}

.order-card:hover::before {
  transform: scaleX(1);
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
  font-size: 1.5rem;
  font-weight: 700;
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
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

.order-actions {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 2px solid #e0e0e0;
  background: linear-gradient(to bottom, transparent, rgba(102, 126, 234, 0.03));
  border-radius: 0 0 12px 12px;
  padding-left: 0;
  padding-right: 0;
  padding-bottom: 0.5rem;
}

.order-actions h4 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1rem;
}

.action-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: center;
}

.action-buttons .btn {
  min-width: 160px;
}

.final-status-message {
  color: #666;
  font-style: italic;
  margin: 0;
  padding: 0.5rem 0;
}
</style>



