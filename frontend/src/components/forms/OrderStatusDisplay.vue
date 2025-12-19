<template>
  <span :class="['order-status', `status-${status.toLowerCase()}`]">
    {{ status }}
  </span>
</template>

<script setup lang="ts">
import { OrderStatus } from '../../services/api/types'

interface Props {
  status: OrderStatus
}

defineProps<Props>()
</script>

<style scoped>
.order-status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.875rem;
  font-weight: 500;
  text-transform: capitalize;
}

.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.status-paid {
  background-color: #d1ecf1;
  color: #0c5460;
}

.status-shipped {
  background-color: #cce5ff;
  color: #004085;
}

.status-delivered {
  background-color: #d4edda;
  color: #155724;
}

.status-cancelled {
  background-color: #f8d7da;
  color: #721c24;
}
</style>



