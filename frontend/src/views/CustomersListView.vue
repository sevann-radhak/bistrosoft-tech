<template>
  <div class="customers-list-view">
    <div class="header-section">
      <h1>All Customers</h1>
      <p class="subtitle">Manage your customer database</p>
    </div>
    
    <div class="search-section">
      <input
        v-model="searchQuery"
        type="text"
        class="search-input"
        placeholder="Search customers by name or email..."
        @input="filterCustomers"
      />
    </div>
    
    <div v-if="loading" class="loading">
      <p>Loading customers...</p>
    </div>
    
    <div v-else-if="error" class="error">
      <ErrorMessage :message="error" />
      <Button @click="loadCustomers">Retry</Button>
    </div>
    
    <div v-else-if="filteredCustomers.length === 0" class="empty-state">
      <p v-if="searchQuery">No customers found matching "{{ searchQuery }}"</p>
      <p v-else>No customers registered yet</p>
      <Button @click="$router.push('/customers/create')">
        Create First Customer
      </Button>
    </div>
    
    <div v-else class="customers-grid">
      <div
        v-for="customer in filteredCustomers"
        :key="customer.id"
        class="customer-card"
        @click="viewCustomer(customer.id)"
      >
        <div class="customer-header">
          <h3>{{ customer.name }}</h3>
          <span class="customer-badge" v-if="customer.orders && customer.orders.length > 0">
            {{ customer.orders.length }} order{{ customer.orders.length !== 1 ? 's' : '' }}
          </span>
        </div>
        <div class="customer-details">
          <p class="customer-email">📧 {{ customer.email }}</p>
          <p v-if="customer.phoneNumber" class="customer-phone">📞 {{ customer.phoneNumber }}</p>
        </div>
        <div class="customer-actions">
          <Button
            size="small"
            @click.stop="viewCustomer(customer.id)"
          >
            View Details
          </Button>
          <Button
            variant="secondary"
            size="small"
            @click.stop="$router.push(`/customers/${customer.id}/orders`)"
          >
            View Orders
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCustomerStore } from '../stores'
import { Button, ErrorMessage } from '../components/common'
import type { CustomerDto } from '../services/api/types'

const router = useRouter()
const customerStore = useCustomerStore()

const searchQuery = ref('')
const loading = ref(false)
const error = ref<string | null>(null)

const filteredCustomers = computed(() => {
  if (!searchQuery.value.trim()) {
    return customerStore.customers
  }
  
  const query = searchQuery.value.toLowerCase()
  return customerStore.customers.filter(c =>
    c.name.toLowerCase().includes(query) ||
    c.email.toLowerCase().includes(query) ||
    (c.phoneNumber && c.phoneNumber.includes(query))
  )
})

onMounted(async () => {
  await loadCustomers()
})

async function loadCustomers(): Promise<void> {
  loading.value = true
  error.value = null
  
  try {
    await customerStore.fetchAllCustomers()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load customers'
    console.error('Failed to load customers:', err)
  } finally {
    loading.value = false
  }
}

function filterCustomers(): void {
  // Filtering is handled by computed property
}

function viewCustomer(id: string): void {
  router.push(`/customers/${id}`)
}
</script>

<style scoped>
.customers-list-view {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.header-section {
  text-align: center;
  margin-bottom: 2rem;
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

.search-section {
  margin-bottom: 2rem;
}

.search-input {
  width: 100%;
  padding: 1rem;
  font-size: 1.125rem;
  border: 2px solid #ddd;
  border-radius: 12px;
  transition: all 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
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

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.empty-state p {
  margin-bottom: 1rem;
  font-size: 1.125rem;
}

.customers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
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
  margin-bottom: 1rem;
}

.customer-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.25rem;
}

.customer-badge {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0.375rem 0.875rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.customer-details {
  margin-bottom: 1rem;
}

.customer-email,
.customer-phone {
  margin: 0.5rem 0;
  color: #666;
  font-size: 0.875rem;
}

.customer-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
}

@media (max-width: 768px) {
  .customers-grid {
    grid-template-columns: 1fr;
  }
}
</style>
