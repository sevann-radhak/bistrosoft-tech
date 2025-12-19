import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'

const getBaseURL = (): string => {
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL
  }
  
  if (import.meta.env.PROD) {
    return '/api'
  }
  
  return 'http://localhost:5000/api'
}

const apiClient: AxiosInstance = axios.create({
  baseURL: getBaseURL(),
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
      const data = error.response.data as any
      
      let errorMessage = 'An error occurred'
      let errorDetails: { title?: string; errors?: Record<string, string[]> } = {}
      
      if (data) {
        if (data.detail) {
          errorMessage = data.detail
        } else if (data.message) {
          errorMessage = data.message
        }
        
        if (data.title) {
          errorDetails.title = data.title
        }
        
        if (data.errors) {
          errorDetails.errors = data.errors
          
          if (Object.keys(data.errors).length > 0) {
            const firstField = Object.keys(data.errors)[0]
            const firstError = data.errors[firstField]?.[0]
            if (firstError) {
              errorMessage = `${firstField}: ${firstError}`
            }
          }
        }
      }
      
      const enhancedError = new Error(errorMessage) as Error & { 
        title?: string
        status?: number
        errors?: Record<string, string[]>
      }
      
      enhancedError.title = errorDetails.title
      enhancedError.status = status
      enhancedError.errors = errorDetails.errors
      
      switch (status) {
        case 400:
          break
        case 401:
          localStorage.removeItem('authToken')
          if (!errorMessage.includes('Unauthorized')) {
            enhancedError.message = 'Your session has expired. Please log in again.'
          }
          break
        case 403:
          if (!errorMessage.includes('Forbidden')) {
            enhancedError.message = 'You do not have permission to perform this action.'
          }
          break
        case 404:
          if (!errorMessage.includes('not found')) {
            enhancedError.message = 'The requested resource was not found.'
          }
          break
        case 405:
          enhancedError.message = 'This action is not allowed.'
          break
        case 422:
          if (!errorMessage.includes('Validation')) {
            enhancedError.message = errorMessage || 'Please check your input and try again.'
          }
          break
        case 500:
          enhancedError.message = 'A server error occurred. Please try again later.'
          break
        default:
          break
      }
      
      throw enhancedError
    } else if (error.request) {
      throw new Error('Unable to connect to the server. Please check your internet connection and try again.')
    } else {
      throw new Error('An unexpected error occurred. Please try again.')
    }
  }
)

export default apiClient



