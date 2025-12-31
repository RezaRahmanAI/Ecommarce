import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { combineLatest, map } from 'rxjs';

import { AuthStateService } from '../../../../core/services/auth-state.service';
import { OrderService } from '../../../../core/services/order.service';
import { Order } from '../../../../core/models/order';

@Component({
  selector: 'app-orders-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './orders-page.component.html',
  styleUrl: './orders-page.component.css',
})
export class OrdersPageComponent {
  private readonly authState = inject(AuthStateService);
  private readonly orderService = inject(OrderService);

  readonly orders$ = combineLatest([this.authState.user$, this.orderService.orders$]).pipe(
    map(([user, orders]) => {
      if (!user) {
        return [];
      }
      return orders.filter((order) => order.userId === user.id || order.email === user.email);
    }),
  );

  trackOrder(_: number, order: Order): string {
    return order.id;
  }
}
