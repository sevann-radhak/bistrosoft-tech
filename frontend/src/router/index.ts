import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomeView
    },
    {
      path: '/customers/create',
      name: 'CreateCustomer',
      component: () => import('../views/CreateCustomerView.vue')
    },
    {
      path: '/customers',
      name: 'CustomersList',
      component: () => import('../views/CustomersListView.vue')
    },
    {
      path: '/customers/:id',
      name: 'CustomerDetail',
      component: () => import('../views/CustomerDetailView.vue')
    },
    {
      path: '/customers/:id/orders',
      name: 'CustomerOrders',
      component: () => import('../views/CustomerOrdersView.vue')
    },
    {
      path: '/orders/create',
      name: 'CreateOrder',
      component: () => import('../views/CreateOrderView.vue')
    },
    {
      path: '/products/create',
      name: 'CreateProduct',
      component: () => import('../views/CreateProductView.vue')
    }
  ]
})

export default router



