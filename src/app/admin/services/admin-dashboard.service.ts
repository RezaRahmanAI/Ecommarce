import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import {
  DashboardStats,
  OrderItem,
  PopularProduct,
} from '../models/admin-dashboard.models';

@Injectable({
  providedIn: 'root',
})
export class AdminDashboardService {
  getStats(): Observable<DashboardStats> {
    return of({
      totalRevenue: '৳45,231',
      revenueTrend: '+12.5%',
      newOrders: '1,204',
      ordersTrend: '+5.2%',
      activeCustomers: '892',
      customersTrend: '+1.8%',
      lowStock: '12',
      lowStockTrend: '-2 items',
    });
  }

  getRecentOrders(): Observable<OrderItem[]> {
    return of([
      {
        id: '#ORD-001',
        customerName: 'Sarah Ahmed',
        date: 'Oct 24, 2023',
        amount: '৳120.50',
        status: 'Completed',
      },
      {
        id: '#ORD-002',
        customerName: 'Mohammed Ali',
        date: 'Oct 24, 2023',
        amount: '৳85.00',
        status: 'Pending',
      },
      {
        id: '#ORD-003',
        customerName: 'Fatima Hassan',
        date: 'Oct 23, 2023',
        amount: '৳210.00',
        status: 'Shipped',
      },
      {
        id: '#ORD-004',
        customerName: 'Omar Khalid',
        date: 'Oct 23, 2023',
        amount: '৳55.00',
        status: 'Completed',
      },
    ]);
  }

  getPopularProducts(): Observable<PopularProduct[]> {
    return of([
      {
        id: 'prod-1',
        name: 'Silk Abaya - Midnight Blue',
        soldCount: '245 sold',
        price: '৳89.00',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuAxrZQWnM7iKos_l4vnVGqGRkiCQ0rLyMavRpdR89YjdK-Bt2LIVx9pPo4zH1HLvXR97h0b94ngKJl5qVlBYMjFH4F0RCTR91Fw3CfVkuuOORBXaYdMiMdCfeNjcOGwmU28UoqYBvH8UsMuOiVrIUvYsJ8VrY8FGEbTP8Omoo9J6wVAr_qNiKXffsFdzz-ePVCR7YA-jJ0zzXjs1xhhiAoiQCZp85rU5WHbBUMdF19w7Ul7cIAe903Oi54Nuo9bDpYR1QmYB8NlL78',
      },
      {
        id: 'prod-2',
        name: 'Chiffon Hijab - Sage Green',
        soldCount: '189 sold',
        price: '৳15.00',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuAtnssyIvwDdFNiOBIQNgi4x3dioCRMVejmhu1wCg2T5Li44hUkbWk3A9sIfpUc6eMUhSXE2PGmdeHlkLsXRuhlJSRMShLH1gcGD3PG5B9zANEcN3gjfxMfpR8SrjdzKy3wfpgHHAFi_lFiAqPqrQUE6acDfBDr3Eg-EjBS77Z6Q_x_Oi7uaXBiuNzErMMP-7dDcUzre9nw4EJ6NuquxP7U0GL0e8-jFR7-yvm0hbtvJOBqi02h6WdNNP8FzOilpuMhmzWMa4ziLXE',
      },
      {
        id: 'prod-3',
        name: 'Modest Maxi Dress - Floral',
        soldCount: '134 sold',
        price: '৳65.00',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuCuKPDDffspQpM8qil8ta0ow28lE1uCcnqxWSUxb64Bd32PDZfDb6AJJFGK_oiXU56sJxzL69q65ipOzMgbh7VKNVAIIemu4uTuPxCd96uHXBBIIeIxRVLLXLr6vRNX0cNoXvZFOWsqmVbWbmLb-6o09iyT9NUub1xnGtACAZwJQjxWKnJjaCyqVvDDD9LCZdE1FY5BMukIgYBzruYVxOFDqe3CV_XkRrtupz2LAT0z2aZF-XXxLoD_-WIa0Jw_PC2w_UGxOEKR8vg',
      },
      {
        id: 'prod-4',
        name: 'Cotton Jersey Hijab - Beige',
        soldCount: '98 sold',
        price: '৳12.00',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuA6L5T_s7B8YvLIvn_-ItmqwJMlYvNaMU3KY6ns2JErh5RPDC3BNu0At0EeyD_Wkljk_WnDk0fqtT5HxpKfChW9EVNTgOvVS6uplTawyNJALJfuXwQGg6tw0sDzyg1h4vMyFHtcLZL7gwCezqNQNLrbjjvZctMicZB_whHVODe8fAQUMMq8igS8iQAL9WTwFje5ketJRA6AqddFrVLFartcqV_mPuiMwL2ONasYZb4AUGi6oyivmkrS9zTACAChvbbocKmkqAZab1k',
      },
    ]);
  }
}
