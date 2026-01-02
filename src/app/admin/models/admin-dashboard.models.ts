export interface DashboardStats {
  totalRevenue: string;
  revenueTrend: string;
  newOrders: string;
  ordersTrend: string;
  activeCustomers: string;
  customersTrend: string;
  lowStock: string;
  lowStockTrend: string;
}

export interface OrderItem {
  id: string;
  customerName: string;
  date: string;
  amount: string;
  status: 'Completed' | 'Pending' | 'Shipped';
}

export interface PopularProduct {
  id: string;
  name: string;
  soldCount: string;
  price: string;
  imageUrl: string;
}
