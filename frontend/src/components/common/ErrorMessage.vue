<template>
  <div v-if="message" class="error-message" role="alert">
    <div class="error-icon">
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
      </svg>
    </div>
    <div class="error-content">
      <div v-if="title" class="error-title">{{ title }}</div>
      <div class="error-detail">{{ message }}</div>
      <div v-if="fieldErrors && Object.keys(fieldErrors).length > 0" class="error-fields">
        <ul>
          <li v-for="(errors, field) in fieldErrors" :key="field">
            <strong>{{ formatFieldName(field) }}:</strong>
            <template v-for="(error, index) in errors" :key="index">
              <span>{{ error }}</span><span v-if="index < errors.length - 1">, </span>
            </template>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  message?: string | null
  title?: string | null
  fieldErrors?: Record<string, string[]> | null
}

const props = defineProps<Props>()

function formatFieldName(field: string): string {
  return field
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, str => str.toUpperCase())
    .trim()
}
</script>

<style scoped>
.error-message {
  display: flex;
  gap: 1rem;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, #fff5f5 0%, #ffe5e5 100%);
  border-left: 4px solid #e74c3c;
  border-radius: 8px;
  color: #c0392b;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 12px rgba(231, 76, 60, 0.15);
  animation: slideIn 0.3s ease-out;
}

.error-icon {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  color: #e74c3c;
  margin-top: 2px;
}

.error-icon svg {
  width: 100%;
  height: 100%;
}

.error-content {
  flex: 1;
  min-width: 0;
}

.error-title {
  font-weight: 600;
  font-size: 1rem;
  margin-bottom: 0.5rem;
  color: #a93226;
}

.error-detail {
  font-size: 0.9375rem;
  line-height: 1.5;
  color: #c0392b;
  margin-bottom: 0.5rem;
}

.error-fields {
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid rgba(231, 76, 60, 0.2);
}

.error-fields ul {
  margin: 0;
  padding-left: 1.25rem;
  list-style-type: disc;
}

.error-fields li {
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  line-height: 1.5;
}

.error-fields li:last-child {
  margin-bottom: 0;
}

.error-fields strong {
  color: #a93226;
  font-weight: 600;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>



