<template>
  <div class="home-view">
    <div class="header-section">
      <h1>Bistrosoft Store</h1>
      <p class="subtitle">Manage customers and orders</p>
    </div>
    
    <div class="quick-actions">
      <Button size="large" @click="$router.push('/customers/create')">
        + Create Customer
      </Button>
      <Button size="large" @click="$router.push('/orders/create')">
        + Create Order
      </Button>
      <Button variant="secondary" size="large" @click="$router.push('/customers')">
        👥 View All Customers
      </Button>
    </div>
    
    <div v-if="loading" class="loading">
      <p>Loading customers...</p>
    </div>
    
    <div v-else-if="customerStore.customers.length > 0" class="customers-section">
      <div class="section-header">
        <h2>Recent Customers</h2>
        <Button variant="secondary" size="small" @click="$router.push('/customers')">
          View All
        </Button>
      </div>
      
      <div class="customers-grid">
        <div
          v-for="customer in customerStore.customers.slice(0, 6)"
          :key="customer.id"
          class="customer-card"
          @click="viewCustomer(customer.id)"
        >
          <div class="customer-header">
            <h3>{{ customer.name }}</h3>
            <span v-if="customer.orders && customer.orders.length > 0" class="order-count">
              {{ customer.orders.length }}
            </span>
          </div>
          <p class="customer-email">{{ customer.email }}</p>
          <div class="customer-actions">
            <Button size="small" @click.stop="viewCustomer(customer.id)">
              View
            </Button>
            <Button
              variant="secondary"
              size="small"
              @click.stop="$router.push(`/customers/${customer.id}/orders`)"
            >
              Orders
            </Button>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="empty-state">
      <p>No customers registered yet</p>
      <Button size="large" @click="$router.push('/customers/create')">
        Create First Customer
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCustomerStore } from '../stores'
import { Button } from '../components/common'

const router = useRouter()
const customerStore = useCustomerStore()
const loading = ref(false)

onMounted(async () => {
  if (customerStore.customers.length === 0) {
    loading.value = true
    try {
      await customerStore.fetchAllCustomers()
    } catch (error) {
      console.error('Failed to fetch customers:', error)
    } finally {
      loading.value = false
    }
  }
})

function viewCustomer(id: string): void {
  router.push(`/customers/${id}`)
}
</script>

<style scoped>
.home-view {
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
  color: #7f8c8d;
  font-size: 1.125rem;
}

.quick-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
  margin-bottom: 3rem;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.customers-section {
  margin-top: 2rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h2 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.75rem;
}

.customers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.customer-card {
  padding: 1.75rem;
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 16px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.customer-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 4px;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleY(0);
  transition: transform 0.3s ease;
}

.customer-card:hover {
  border-color: #667eea;
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.2);
  transform: translateY(-4px);
}

.customer-card:hover::before {
  transform: scaleY(1);
}

.customer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.customer-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.125rem;
}

.order-count {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0.375rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 700;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.customer-email {
  color: #666;
  font-size: 0.875rem;
  margin: 0.5rem 0 1rem 0;
}

.customer-actions {
  display: flex;
  gap: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #666;
}

.empty-state p {
  margin-bottom: 1.5rem;
  font-size: 1.125rem;
}

@media (max-width: 768px) {
  .customers-grid {
    grid-template-columns: 1fr;
  }
  
  .quick-actions {
    flex-direction: column;
  }
  
  .quick-actions button {
    width: 100%;
  }
}
</style>



