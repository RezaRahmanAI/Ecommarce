import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

interface AdminNavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.component.html',
})
export class AdminSidebarComponent {
  topItems: AdminNavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/admin/dashboard' },
  ];

  navItems: AdminNavItem[] = [
    { label: 'Orders', icon: 'inventory_2', route: '/admin/orders' },
    { label: 'Blog', icon: 'article', route: '/admin/blog' },
    { label: 'Customers', icon: 'group', route: '/admin/customers' },
    { label: 'Analytics', icon: 'analytics', route: '/admin/analytics' },
  ];

  bottomItems: AdminNavItem[] = [
    { label: 'Settings', icon: 'settings', route: '/admin/settings' },
  ];
}
