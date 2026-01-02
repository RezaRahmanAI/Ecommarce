import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { Order, OrderStatus, OrdersQueryParams } from '../models/orders.models';

const daysAgo = (days: number): string => {
  const date = new Date();
  date.setDate(date.getDate() - days);
  return date.toISOString().slice(0, 10);
};

const MOCK_ORDERS: Order[] = [
  {
    id: 1,
    orderId: '#ORD-7782',
    customerName: 'Ayesha Khan',
    customerInitials: 'AK',
    date: daysAgo(1),
    itemsCount: 3,
    total: 145.0,
    status: 'Processing',
  },
  {
    id: 2,
    orderId: '#ORD-7781',
    customerName: 'Fatima Ahmed',
    customerInitials: 'FA',
    date: daysAgo(2),
    itemsCount: 1,
    total: 89.5,
    status: 'Shipped',
  },
  {
    id: 3,
    orderId: '#ORD-7780',
    customerName: 'Zainab Malik',
    customerInitials: 'ZM',
    date: daysAgo(2),
    itemsCount: 5,
    total: 210.0,
    status: 'Delivered',
  },
  {
    id: 4,
    orderId: '#ORD-7779',
    customerName: 'Omar Farooq',
    customerInitials: 'OF',
    date: daysAgo(3),
    itemsCount: 1,
    total: 55.0,
    status: 'Cancelled',
  },
  {
    id: 5,
    orderId: '#ORD-7778',
    customerName: 'Noura Bashir',
    customerInitials: 'NB',
    date: daysAgo(4),
    itemsCount: 2,
    total: 120.0,
    status: 'Processing',
  },
  {
    id: 6,
    orderId: '#ORD-7777',
    customerName: 'Maryam Yusuf',
    customerInitials: 'MY',
    date: daysAgo(5),
    itemsCount: 4,
    total: 275.0,
    status: 'Delivered',
  },
  {
    id: 7,
    orderId: '#ORD-7776',
    customerName: 'Hassan Ali',
    customerInitials: 'HA',
    date: daysAgo(6),
    itemsCount: 2,
    total: 98.0,
    status: 'Processing',
  },
  {
    id: 8,
    orderId: '#ORD-7775',
    customerName: 'Sara Noor',
    customerInitials: 'SN',
    date: daysAgo(7),
    itemsCount: 1,
    total: 45.0,
    status: 'Refund',
  },
  {
    id: 9,
    orderId: '#ORD-7774',
    customerName: 'Bilal Aziz',
    customerInitials: 'BA',
    date: daysAgo(8),
    itemsCount: 6,
    total: 320.0,
    status: 'Shipped',
  },
  {
    id: 10,
    orderId: '#ORD-7773',
    customerName: 'Iman Rashid',
    customerInitials: 'IR',
    date: daysAgo(9),
    itemsCount: 2,
    total: 110.0,
    status: 'Delivered',
  },
  {
    id: 11,
    orderId: '#ORD-7772',
    customerName: 'Khadija Noor',
    customerInitials: 'KN',
    date: daysAgo(10),
    itemsCount: 3,
    total: 150.0,
    status: 'Processing',
  },
  {
    id: 12,
    orderId: '#ORD-7771',
    customerName: 'Usman Tariq',
    customerInitials: 'UT',
    date: daysAgo(11),
    itemsCount: 1,
    total: 75.0,
    status: 'Cancelled',
  },
  {
    id: 13,
    orderId: '#ORD-7770',
    customerName: 'Rania Saleh',
    customerInitials: 'RS',
    date: daysAgo(12),
    itemsCount: 4,
    total: 210.0,
    status: 'Shipped',
  },
  {
    id: 14,
    orderId: '#ORD-7769',
    customerName: 'Yasmin Rahim',
    customerInitials: 'YR',
    date: daysAgo(13),
    itemsCount: 2,
    total: 130.0,
    status: 'Processing',
  },
  {
    id: 15,
    orderId: '#ORD-7768',
    customerName: 'Salim Hadi',
    customerInitials: 'SH',
    date: daysAgo(14),
    itemsCount: 1,
    total: 60.0,
    status: 'Refund',
  },
  {
    id: 16,
    orderId: '#ORD-7767',
    customerName: 'Lina Qureshi',
    customerInitials: 'LQ',
    date: daysAgo(15),
    itemsCount: 3,
    total: 190.0,
    status: 'Delivered',
  },
  {
    id: 17,
    orderId: '#ORD-7766',
    customerName: 'Faris Zahid',
    customerInitials: 'FZ',
    date: daysAgo(16),
    itemsCount: 2,
    total: 95.0,
    status: 'Processing',
  },
  {
    id: 18,
    orderId: '#ORD-7765',
    customerName: 'Hiba Latif',
    customerInitials: 'HL',
    date: daysAgo(17),
    itemsCount: 5,
    total: 260.0,
    status: 'Shipped',
  },
  {
    id: 19,
    orderId: '#ORD-7764',
    customerName: 'Ola Kareem',
    customerInitials: 'OK',
    date: daysAgo(18),
    itemsCount: 3,
    total: 140.0,
    status: 'Delivered',
  },
  {
    id: 20,
    orderId: '#ORD-7763',
    customerName: 'Samiya Ali',
    customerInitials: 'SA',
    date: daysAgo(19),
    itemsCount: 2,
    total: 105.0,
    status: 'Processing',
  },
  {
    id: 21,
    orderId: '#ORD-7762',
    customerName: 'Musa Ibrahim',
    customerInitials: 'MI',
    date: daysAgo(20),
    itemsCount: 1,
    total: 70.0,
    status: 'Cancelled',
  },
];

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  private orders = [...MOCK_ORDERS];

  getOrders(params: OrdersQueryParams): Observable<{ items: Order[]; total: number }> {
    const filtered = this.applyFilters(params);
    const startIndex = (params.page - 1) * params.pageSize;
    const items = filtered.slice(startIndex, startIndex + params.pageSize);
    return of({ items, total: filtered.length });
  }

  getFilteredOrders(params: OrdersQueryParams): Observable<Order[]> {
    return of(this.applyFilters(params));
  }

  exportOrders(params: OrdersQueryParams): string {
    const rows = this.applyFilters(params);
    const header = ['Order ID', 'Customer Name', 'Date', 'Items', 'Total', 'Status'];
    const csvRows = rows.map((order) => [
      order.orderId,
      order.customerName,
      order.date,
      order.itemsCount.toString(),
      order.total.toFixed(2),
      order.status,
    ]);
    return [header, ...csvRows].map((row) => row.join(',')).join('\n');
  }

  print(): void {
    window.print();
  }

  updateStatus(orderId: number, status: OrderStatus): void {
    this.orders = this.orders.map((order) =>
      order.id === orderId ? { ...order, status } : order,
    );
  }

  private applyFilters(params: OrdersQueryParams): Order[] {
    const searchTerm = params.searchTerm.trim().toLowerCase();
    const now = new Date();

    return this.orders.filter((order) => {
      const matchesSearch =
        !searchTerm ||
        order.orderId.toLowerCase().includes(searchTerm) ||
        order.customerName.toLowerCase().includes(searchTerm);

      const matchesStatus =
        params.status === 'All' ? true : order.status === params.status;

      const orderDate = new Date(order.date);
      let matchesDate = true;
      if (params.dateRange === 'Last 7 Days') {
        const startDate = new Date(now);
        startDate.setDate(now.getDate() - 7);
        matchesDate = orderDate >= startDate;
      } else if (params.dateRange === 'Last 30 Days') {
        const startDate = new Date(now);
        startDate.setDate(now.getDate() - 30);
        matchesDate = orderDate >= startDate;
      } else if (params.dateRange === 'This Year') {
        const startDate = new Date(now.getFullYear(), 0, 1);
        matchesDate = orderDate >= startDate;
      }

      return matchesSearch && matchesStatus && matchesDate;
    });
  }
}
