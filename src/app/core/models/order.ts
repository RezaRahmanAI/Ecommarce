import { ShippingAddress, ShippingMethod } from './checkout';

export enum OrderStatus {
  Confirmed = 'Confirmed',
  Processing = 'Processing',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
}

export interface OrderItem {
  productId: number;
  name: string;
  price: number;
  quantity: number;
  color: string;
  size: string;
  imageUrl: string;
  imageAlt: string;
  sku: string;
}

export interface PaymentInfo {
  brand: string;
  last4: string;
  expMonth: string;
  expYear: string;
}

export interface OrderTotals {
  subtotal: number;
  shipping: number;
  tax: number;
  total: number;
}

export interface OrderTimeline {
  confirmedDate: string;
  processingLabel: string;
  shippedEta: string;
  deliveredEta: string;
}

export interface Order {
  id: string;
  email: string;
  status: OrderStatus;
  items: OrderItem[];
  shippingAddress: ShippingAddress;
  shippingMethod: ShippingMethod;
  payment: PaymentInfo;
  totals: OrderTotals;
  timeline: OrderTimeline;
}
