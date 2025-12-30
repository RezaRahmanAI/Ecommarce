import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { CheckoutState } from '../models/checkout';
import { CartService } from './cart.service';
import { OrderService } from './order.service';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private readonly stateSubject = new BehaviorSubject<CheckoutState>({
    currentStep: 1,
    email: '',
    newsletter: true,
    shippingAddress: {
      firstName: '',
      lastName: '',
      address: '',
      apartment: '',
      city: '',
      region: '',
      postalCode: '',
      country: 'United States',
    },
    shippingMethodId: 'standard',
    promoCode: '',
  });

  readonly state$ = this.stateSubject.asObservable();

  constructor(private readonly cartService: CartService, private readonly orderService: OrderService) {}

  setStep(step: number): void {
    this.stateSubject.next({ ...this.stateSubject.getValue(), currentStep: step });
  }

  updateState(partial: Partial<CheckoutState>): void {
    this.stateSubject.next({ ...this.stateSubject.getValue(), ...partial });
  }

  createOrder(): void {
    const state = this.stateSubject.getValue();
    const cartItems = this.cartService.getCartSnapshot();
    const summary = this.cartService.getSummarySnapshot();
    this.orderService.placeOrder({ state, cartItems, summary });
  }
}
