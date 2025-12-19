<template>
  <div class="customer-selector">
    <div class="search-container">
      <input
        v-model="searchQuery"
        type="text"
        class="search-input"
        placeholder="Search customer by name or email..."
        @input="filterCustomers"
        @focus="showDropdown = true"
      />
      <div v-if="showDropdown && filteredCustomers.length > 0" class="dropdown">
        <div
          v-for="customer in filteredCustomers"
          :key="customer.id"
          class="dropdown-item"
          @click="selectCustomer(customer)"
        >
          <div class="customer-name">{{ customer.name }}</div>
          <div class="customer-email">{{ customer.email }}</div>
        </div>
      </div>
      <div v-if="showDropdown && searchQuery && filteredCustomers.length === 0" class="dropdown">
        <div class="dropdown-item no-results">
          No customers found
        </div>
      </div>
    </div>
    
    <div v-if="selectedCustomer" class="selected-customer">
      <div class="selected-info">
        <strong>{{ selectedCustomer.name }}</strong>
        <span class="selected-email">{{ selectedCustomer.email }}</span>
      </div>
      <button type="button" class="clear-btn" @click="clearSelection">×</button>
    </div>
    
    <div class="actions">
      <Button
        type="button"
        variant="secondary"
        size="small"
        @click="$router.push('/customers/create')"
      >
        + New Customer
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCustomerStore } from '../../stores'
import { Button } from '../common'
import type { CustomerDto } from '../../services/api/types'

interface Props {
  modelValue: string
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const router = useRouter()
const customerStore = useCustomerStore()
const searchQuery = ref('')
const showDropdown = ref(false)
const selectedCustomer = ref<CustomerDto | null>(null)

const filteredCustomers = computed(() => {
  if (!searchQuery.value) {
    return customerStore.customers.slice(0, 5)
  }
  const query = searchQuery.value.toLowerCase()
  return customerStore.customers.filter(c =>
    c.name.toLowerCase().includes(query) ||
    c.email.toLowerCase().includes(query)
  ).slice(0, 10)
})

function handleClickOutside(event: MouseEvent): void {
  const target = event.target as HTMLElement
  if (!target.closest('.customer-selector')) {
    showDropdown.value = false
  }
}

onMounted(async () => {
  document.addEventListener('click', handleClickOutside)
  
  if (customerStore.customers.length === 0) {
    await customerStore.fetchAllCustomers()
  }
  
  if (props.modelValue) {
    const customer = customerStore.customerById(props.modelValue)
    if (customer) {
      selectedCustomer.value = customer
      searchQuery.value = customer.name
    }
  }
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

watch(() => props.modelValue, (newValue) => {
  if (newValue && !selectedCustomer.value) {
    const customer = customerStore.customerById(newValue)
    if (customer) {
      selectedCustomer.value = customer
      searchQuery.value = customer.name
    }
  } else if (!newValue) {
    selectedCustomer.value = null
    searchQuery.value = ''
  }
})

function selectCustomer(customer: CustomerDto): void {
  selectedCustomer.value = customer
  searchQuery.value = customer.name
  showDropdown.value = false
  emit('update:modelValue', customer.id)
}

function clearSelection(): void {
  selectedCustomer.value = null
  searchQuery.value = ''
  showDropdown.value = false
  emit('update:modelValue', '')
}

function filterCustomers(): void {
  showDropdown.value = true
}

</script>

<style scoped>
.customer-selector {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.search-container {
  position: relative;
}

.search-input {
  width: 100%;
  padding: 1rem;
  font-size: 1.125rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  transition: all 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 0.25rem;
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  max-height: 300px;
  overflow-y: auto;
  z-index: 1000;
}

.dropdown-item {
  padding: 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
  border-bottom: 1px solid #f0f0f0;
}

.dropdown-item:last-child {
  border-bottom: none;
}

.dropdown-item:hover {
  background-color: #f8f9fa;
}

.dropdown-item.no-results {
  color: #999;
  cursor: default;
}

.dropdown-item.no-results:hover {
  background-color: white;
}

.customer-name {
  font-weight: 600;
  color: #333;
  margin-bottom: 0.25rem;
}

.customer-email {
  font-size: 0.875rem;
  color: #666;
}

.selected-customer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #e8f5e9;
  border: 2px solid #4caf50;
  border-radius: 8px;
}

.selected-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.selected-info strong {
  color: #2e7d32;
  font-size: 1.125rem;
}

.selected-email {
  color: #666;
  font-size: 0.875rem;
}

.clear-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #666;
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  line-height: 1;
  transition: color 0.2s;
}

.clear-btn:hover {
  color: #e74c3c;
}

.actions {
  display: flex;
  justify-content: flex-end;
}
</style>
