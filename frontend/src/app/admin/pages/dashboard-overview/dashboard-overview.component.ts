import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Observable, firstValueFrom } from 'rxjs';

import {
  DashboardStats,
  OrderItem,
  PopularProduct,
} from '../../models/admin-dashboard.models';
import { AdminDashboardService } from '../../services/admin-dashboard.service';
import { PriceDisplayComponent } from '../../../shared/components/price-display/price-display.component';

@Component({
  selector: 'app-dashboard-overview',
  standalone: true,
  imports: [CommonModule, RouterModule, PriceDisplayComponent],
  templateUrl: './dashboard-overview.component.html',
})
export class DashboardOverviewComponent {
  private adminDashboardService = inject(AdminDashboardService);

  stats$: Observable<DashboardStats> = this.adminDashboardService.getStats();
  recentOrders$: Observable<OrderItem[]> = this.adminDashboardService.getRecentOrders();
  popularProducts$: Observable<PopularProduct[]> =
    this.adminDashboardService.getPopularProducts();

  timeRanges = ['Last 30 Days', 'Last 7 Days', 'This Year'];
  selectedRange = this.timeRanges[0];

  onRangeChange(value: string): void {
    this.selectedRange = value;
  }

  async exportRevenueReport(): Promise<void> {
    const stats = await firstValueFrom(this.stats$);
    const csvRows = [
      ['Range', 'Metric', 'Value'],
      [this.selectedRange, 'Total Order Value', stats.totalRevenue.toFixed(2)],
      [this.selectedRange, 'Total Orders', stats.totalOrders],
      [this.selectedRange, 'Delivered Orders', stats.deliveredOrders],
      [this.selectedRange, 'Orders Left', stats.ordersLeft],
      [this.selectedRange, 'Returned Orders', stats.returnedOrders],
      [this.selectedRange, 'Customer Queries', stats.customerQueries],
    ];

    const csvContent = csvRows.map((row) => row.join(',')).join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'revenue-report.csv';
    link.click();
    URL.revokeObjectURL(url);
  }

  statusClass(status: OrderItem['status']): string {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Shipped':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }
}
