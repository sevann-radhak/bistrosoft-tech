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
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  text-transform: capitalize;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  letter-spacing: 0.5px;
}

.status-pending {
  background: linear-gradient(135deg, #fff3cd 0%, #ffe69c 100%);
  color: #856404;
  border: 1px solid #ffc107;
}

.status-paid {
  background: linear-gradient(135deg, #d1ecf1 0%, #bee5eb 100%);
  color: #0c5460;
  border: 1px solid #17a2b8;
}

.status-shipped {
  background: linear-gradient(135deg, #cce5ff 0%, #b3d9ff 100%);
  color: #004085;
  border: 1px solid #0066cc;
}

.status-delivered {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  color: #155724;
  border: 1px solid #28a745;
}

.status-cancelled {
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  color: #721c24;
  border: 1px solid #dc3545;
}
</style>



