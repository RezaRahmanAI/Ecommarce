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

export interface CustomerDetails {
  name: string;
  phone: string;
  address: string;
  deliveryDetails: string;
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
  status: OrderStatus;
  items: OrderItem[];
  customer: CustomerDetails;
  totals: OrderTotals;
  timeline: OrderTimeline;
}
