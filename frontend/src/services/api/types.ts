export interface ApiError {
  message: string
  errors?: Record<string, string[]>
}

export interface CustomerDto {
  id: string
  name: string
  email: string
  phoneNumber?: string
  orders?: OrderDto[]
}

export interface CreateCustomerDto {
  name: string
  email: string
  phoneNumber?: string
}

export interface OrderDto {
  id: string
  customerId: string
  totalAmount: number
  createdAt: string
  status: OrderStatus
  orderItems: OrderItemDto[]
}

export interface CreateOrderDto {
  customerId: string
  items: CreateOrderItemDto[]
}

export interface CreateOrderItemDto {
  productId: string
  quantity: number
}

export interface OrderItemDto {
  id: string
  orderId: string
  productId: string
  quantity: number
  unitPrice: number
  product?: ProductDto
}

export interface ProductDto {
  id: string
  name: string
  price: number
  stockQuantity: number
}

export interface UpdateOrderStatusDto {
  status: OrderStatus
}

export enum OrderStatus {
  Pending = 'Pending',
  Paid = 'Paid',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled'
}



