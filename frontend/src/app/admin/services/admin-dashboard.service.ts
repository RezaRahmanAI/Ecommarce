import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import {
  DashboardStats,
  OrderItem,
  PopularProduct,
} from '../models/admin-dashboard.models';
import { ApiHttpClient } from '../../core/http/http-client';

@Injectable({
  providedIn: 'root',
})
export class AdminDashboardService {
  private readonly api = inject(ApiHttpClient);

  getStats(): Observable<DashboardStats> {
    return this.api.get<DashboardStats>('/admin/dashboard/stats');
  }

  getRecentOrders(): Observable<OrderItem[]> {
    return this.api.get<OrderItem[]>('/admin/dashboard/orders/recent');
  }

  getPopularProducts(): Observable<PopularProduct[]> {
    return this.api.get<PopularProduct[]>('/admin/dashboard/products/popular');
  }
}
