export type OrderStatus = 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled' | 'Refund';

export interface Order {
  id: number;
  orderId: string;
  customerName: string;
  customerInitials: string;
  date: string;
  itemsCount: number;
  total: number;
  deliveryDetails: string;
  status: OrderStatus;
}

export interface OrdersQueryParams {
  searchTerm: string;
  status: 'All' | OrderStatus;
  dateRange: 'Last 7 Days' | 'Last 30 Days' | 'This Year' | 'All Time';
  page: number;
  pageSize: number;
}
