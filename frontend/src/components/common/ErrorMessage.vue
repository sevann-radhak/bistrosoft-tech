<template>
  <div v-if="message" class="error-message">
    {{ message }}
  </div>
</template>

<script setup lang="ts">
interface Props {
  message?: string | null
}

defineProps<Props>()
</script>

<style scoped>
.error-message {
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, #fee 0%, #fdd 100%);
  border: 2px solid #e74c3c;
  border-radius: 12px;
  color: #c0392b;
  margin-bottom: 1.5rem;
  font-weight: 500;
  box-shadow: 0 4px 12px rgba(231, 76, 60, 0.2);
  animation: shake 0.5s ease;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}
</style>



