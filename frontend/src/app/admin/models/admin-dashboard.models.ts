export interface DashboardStats {
  totalRevenue: number;
  totalOrders: string;
  deliveredOrders: string;
  ordersLeft: string;
  returnedOrders: string;
  customerQueries: string;
  totalPurchaseCost: number;
  averageSellingPrice: number;
  returnValue: number;
  returnRate: string;
}

export interface OrderItem {
  id: string;
  customerName: string;
  date: string;
  amount: number;
  status: 'Completed' | 'Pending' | 'Shipped';
}

export interface PopularProduct {
  id: string;
  name: string;
  soldCount: string;
  price: number;
  imageUrl: string;
}
