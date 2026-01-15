import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';

import { MOCK_ORDERS } from '../data/mock-orders';
import { CartItem, CartSummary } from '../models/cart';
import { CheckoutState } from '../models/checkout';
import { Order, OrderItem, OrderStatus } from '../models/order';
import { CustomerOrderApiService, CustomerOrderResponse } from './customer-order-api.service';

interface PlaceOrderPayload {
  state: CheckoutState;
  cartItems: CartItem[];
  summary: CartSummary;
}

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private readonly customerOrderApi = inject(CustomerOrderApiService);
  private readonly storageKey = 'orders';
  private readonly ordersSubject = new BehaviorSubject<Order[]>(this.loadOrders());
  readonly orders$ = this.ordersSubject.asObservable();

  getOrderById(orderId: string): Observable<Order | undefined> {
    return this.orders$.pipe(map((orders) => orders.find((order) => order.id === orderId)));
  }

  getFallbackOrder(): Order {
    return this.ordersSubject.getValue()[0];
  }

  placeOrder(payload: PlaceOrderPayload): Observable<Order> {
    const items: OrderItem[] = payload.cartItems.map((item) => ({
      productId: item.productId,
      name: item.name,
      price: item.price,
      quantity: item.quantity,
      color: item.color,
      size: item.size,
      imageUrl: item.imageUrl,
      imageAlt: item.imageAlt,
      sku: `SKU-${item.productId}`,
    }));

    return this.customerOrderApi
      .placeOrder({
        name: payload.state.fullName,
        phone: payload.state.phone,
        address: payload.state.address,
        deliveryDetails: payload.state.deliveryDetails,
        itemsCount: payload.summary.itemsCount,
        total: payload.summary.total,
      })
      .pipe(
        map((response) => this.buildOrder(response, items, payload.summary)),
        map((order) => {
          this.ordersSubject.next([order, ...this.ordersSubject.getValue()]);
          this.persistOrders();
          return order;
        }),
      );
  }

  private loadOrders(): Order[] {
    const stored = localStorage.getItem(this.storageKey);
    if (stored) {
      try {
        return JSON.parse(stored) as Order[];
      } catch {
        return [...MOCK_ORDERS];
      }
    }

    return [...MOCK_ORDERS];
  }

  private persistOrders(): void {
    localStorage.setItem(this.storageKey, JSON.stringify(this.ordersSubject.getValue()));
  }

  private buildOrder(response: CustomerOrderResponse, items: OrderItem[], summary: CartSummary): Order {
    return {
      id: response.orderId,
      status: OrderStatus.Confirmed,
      items,
      customer: {
        name: response.name,
        phone: response.phone,
        address: response.address,
        deliveryDetails: response.deliveryDetails,
      },
      totals: {
        subtotal: summary.subtotal,
        shipping: summary.shipping,
        tax: summary.tax,
        total: summary.total,
      },
      timeline: {
        confirmedDate: 'Today',
        processingLabel: 'Pending',
        shippedEta: 'Est. in 2-3 days',
        deliveredEta: 'Est. in 4-6 days',
      },
    };
  }
}
