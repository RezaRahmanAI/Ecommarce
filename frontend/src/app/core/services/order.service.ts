import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';

import { MOCK_ORDERS } from '../data/mock-orders';
import { SHIPPING_METHODS } from '../data/mock-shipping-methods';
import { CartItem, CartSummary } from '../models/cart';
import { CheckoutState } from '../models/checkout';
import { Order, OrderItem, OrderStatus } from '../models/order';

interface PlaceOrderPayload {
  state: CheckoutState;
  cartItems: CartItem[];
  summary: CartSummary;
  userId?: string;
}

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private readonly storageKey = 'orders';
  private readonly ordersSubject = new BehaviorSubject<Order[]>(this.loadOrders());
  readonly orders$ = this.ordersSubject.asObservable();

  getOrderById(orderId: string): Observable<Order | undefined> {
    return this.orders$.pipe(map((orders) => orders.find((order) => order.id === orderId)));
  }

  getFallbackOrder(): Order {
    return this.ordersSubject.getValue()[0];
  }

  placeOrder(payload: PlaceOrderPayload): Order {
    const orderId = `MSLM-${Math.floor(100000 + Math.random() * 900000)}`;
    const shippingMethod = SHIPPING_METHODS.find((method) => method.id === payload.state.shippingMethodId) ??
      SHIPPING_METHODS[0];
    const paymentDetails = payload.state.payment;
    const last4 = paymentDetails.cardNumber.slice(-4) || '4242';
    const brand = this.resolveCardBrand(paymentDetails.cardNumber);

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

    const order: Order = {
      id: orderId,
      userId: payload.userId,
      email: payload.state.email,
      status: OrderStatus.Confirmed,
      items,
      shippingAddress: payload.state.shippingAddress,
      shippingMethod,
      payment: {
        brand,
        last4,
        expMonth: paymentDetails.expMonth || '12',
        expYear: paymentDetails.expYear || '25',
      },
      totals: {
        subtotal: payload.summary.subtotal,
        shipping: shippingMethod.price,
        tax: payload.summary.tax,
        total: payload.summary.subtotal + payload.summary.tax + shippingMethod.price,
      },
      timeline: {
        confirmedDate: 'Today',
        processingLabel: 'Pending',
        shippedEta: 'Est. in 2-3 days',
        deliveredEta: 'Est. in 4-6 days',
      },
    };

    this.ordersSubject.next([order, ...this.ordersSubject.getValue()]);
    this.persistOrders();
    return order;
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

  private resolveCardBrand(cardNumber: string): string {
    if (cardNumber.startsWith('4')) {
      return 'Visa';
    }
    if (cardNumber.startsWith('5')) {
      return 'Mastercard';
    }
    if (cardNumber.startsWith('3')) {
      return 'Amex';
    }
    return 'Card';
  }
}
