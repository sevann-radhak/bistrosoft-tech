import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'

const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
})

apiClient.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    const token = localStorage.getItem('authToken')
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error: AxiosError) => {
    return Promise.reject(error)
  }
)

apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response.data
  },
  (error: AxiosError) => {
    if (error.response) {
      const status = error.response.status
      const data = error.response.data as { message?: string; errors?: Record<string, string[]> }
      
      switch (status) {
        case 400:
          throw new Error(data.message || 'Bad request')
        case 401:
          localStorage.removeItem('authToken')
          throw new Error('Unauthorized')
        case 403:
          throw new Error('Forbidden')
        case 404:
          throw new Error('Resource not found')
        case 422:
          throw new Error(data.message || 'Validation error')
        case 500:
          throw new Error('Internal server error')
        default:
          throw new Error(data.message || 'An error occurred')
      }
    } else if (error.request) {
      throw new Error('Network error. Please check your connection.')
    } else {
      throw new Error('An unexpected error occurred')
    }
  }
)

export default apiClient
