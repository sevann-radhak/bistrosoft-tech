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
  padding: 0.75rem;
  background-color: #fee;
  border: 1px solid #e74c3c;
  border-radius: 4px;
  color: #c0392b;
  margin-bottom: 1rem;
}
</style>



