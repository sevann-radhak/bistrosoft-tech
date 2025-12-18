---
alwaysApply: true
---

# Frontend Technical Rules - Vue.js 3

## Vue.js 3 Requirements

- **Version**: Vue.js 3.x (latest stable)
- **Build Tool**: Vite (recommended) or Vue CLI
- **Package Manager**: npm or yarn
- **TypeScript**: Optional but recommended

## Project Structure

```
frontend/
├── src/
│   ├── components/
│   │   ├── common/
│   │   └── forms/
│   ├── views/
│   ├── stores/ (Pinia)
│   ├── services/
│   │   └── api/
│   ├── router/
│   ├── composables/
│   ├── types/
│   └── App.vue
├── public/
└── package.json
```

## Composition API

- Use **Composition API** (preferred over Options API)
- Use `<script setup>` syntax
- Use `ref` for reactive primitives
- Use `reactive` for objects
- Use `computed` for derived state
- Use `watch` or `watchEffect` for side effects

## Component Structure

### Template
- Use semantic HTML
- Use Vue directives appropriately (`v-if`, `v-for`, `v-bind`, `v-on`)
- Use `:key` with `v-for`
- Keep templates readable and maintainable

### Script
- Use `<script setup>` syntax
- Import composables and utilities
- Define props with `defineProps<T>()`
- Define emits with `defineEmits<T>()`
- Use TypeScript interfaces for type safety

### Style
- Use scoped styles: `<style scoped>`
- Consider CSS modules for complex styling
- Use modern CSS features (CSS Grid, Flexbox)

## State Management

### Pinia (Recommended)
- Install Pinia: `npm install pinia`
- Create stores for:
  - Customers
  - Orders
  - Products
- Use stores for shared state
- Keep components simple, move logic to stores

### Store Structure
```typescript
import { defineStore } from 'pinia'

export const useCustomerStore = defineStore('customer', {
  state: () => ({
    customers: [],
    currentCustomer: null,
    loading: false,
    error: null
  }),
  actions: {
    async fetchCustomer(id: string) {
      // Implementation
    }
  },
  getters: {
    customerById: (state) => (id: string) => {
      return state.customers.find(c => c.id === id)
    }
  }
})
```

## API Integration

### HTTP Client
- Use **Axios** or native `fetch`
- Create a centralized API service
- Configure base URL from environment variables
- Setup request/response interceptors
- Handle errors consistently

### API Service Structure
```typescript
// services/api/client.ts
import axios from 'axios'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor
apiClient.interceptors.request.use(config => {
  // Add auth token if needed
  return config
})

// Response interceptor
apiClient.interceptors.response.use(
  response => response.data,
  error => {
    // Handle errors
    return Promise.reject(error)
  }
)

export default apiClient
```

### Service Methods
```typescript
// services/api/customers.ts
import apiClient from './client'

export const customerService = {
  create: (data: CreateCustomerDto) => apiClient.post('/customers', data),
  getById: (id: string) => apiClient.get(`/customers/${id}`),
  getOrders: (id: string) => apiClient.get(`/customers/${id}/orders`)
}
```

## Routing

### Vue Router
- Install Vue Router: `npm install vue-router@4`
- Define routes in `router/index.ts`
- Use route guards if authentication is implemented
- Use lazy loading for routes: `() => import('./views/OrderView.vue')`

### Route Structure
```typescript
import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/HomeView.vue')
  },
  {
    path: '/customers/:id/orders',
    name: 'CustomerOrders',
    component: () => import('../views/CustomerOrdersView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
```

## Forms and Validation

### Form Handling
- Use `v-model` for two-way binding
- Validate on submit and/or on blur
- Show validation errors clearly
- Disable submit button while processing

### Validation Library (Optional)
- Consider using VeeValidate or similar
- Or implement custom validation logic
- Validate both client-side and rely on server validation

## Error Handling

- Display user-friendly error messages
- Handle network errors gracefully
- Show loading states during API calls
- Use try/catch in async operations

## Environment Configuration

- Use `.env` files for configuration
- Prefix variables with `VITE_` for Vite
- Example: `VITE_API_BASE_URL=http://localhost:5000/api`

## Best Practices

1. **Component Reusability**: Create reusable components
2. **Composables**: Extract reusable logic into composables
3. **Type Safety**: Use TypeScript interfaces for props and data
4. **Performance**: Use `v-show` vs `v-if` appropriately
5. **Accessibility**: Use semantic HTML and ARIA attributes
6. **Code Organization**: Keep components small and focused
7. **Naming**: Use PascalCase for components, camelCase for variables/functions

## Required Views/Pages

1. **Customer Creation Form**
   - Fields: Name, Email, PhoneNumber
   - Validation for required fields and email format
   - Submit to POST /api/customers

2. **Order Creation Form**
   - Customer selection
   - Product selection with quantities
   - Display total amount
   - Submit to POST /api/orders

3. **Customer Orders List**
   - Display all orders for a customer
   - Show order status
   - Show order items and products
   - Fetch from GET /api/customers/{id}/orders

