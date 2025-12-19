<template>
  <span :class="['order-status', `status-${statusString.toLowerCase()}`]">
    {{ statusString }}
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { OrderStatus } from '../../services/api/types'

interface Props {
  status: OrderStatus | string | number
}

const props = defineProps<Props>()

const statusString = computed(() => {
  if (typeof props.status === 'string') {
    return props.status
  }
  
  if (typeof props.status === 'number') {
    const statusMap: Record<number, string> = {
      0: 'Pending',
      1: 'Paid',
      2: 'Shipped',
      3: 'Delivered',
      4: 'Cancelled'
    }
    return statusMap[props.status] || 'Unknown'
  }
  
  return String(props.status)
})
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



