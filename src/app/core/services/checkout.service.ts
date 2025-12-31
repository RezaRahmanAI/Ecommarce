import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { CheckoutState } from '../models/checkout';
import { CartService } from './cart.service';
import { OrderService } from './order.service';
import { AuthStateService } from './auth-state.service';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private readonly storageBaseKey = 'checkout_state';
  private activeStorageKey = this.storageBaseKey;

  private readonly stateSubject = new BehaviorSubject<CheckoutState>(this.buildDefaultState());

  readonly state$ = this.stateSubject.asObservable();

  constructor(
    private readonly cartService: CartService,
    private readonly orderService: OrderService,
    private readonly authState: AuthStateService,
  ) {
    this.authState.user$.subscribe((user) => {
      this.activeStorageKey = user ? `${this.storageBaseKey}_${user.id}` : this.storageBaseKey;
      const storedState = this.loadState();
      this.stateSubject.next(storedState ?? this.buildDefaultState(user?.email ?? ''));
    });

    this.state$.subscribe((state) => {
      localStorage.setItem(this.activeStorageKey, JSON.stringify(state));
    });
  }

  setStep(step: number): void {
    this.stateSubject.next({ ...this.stateSubject.getValue(), currentStep: step });
  }

  updateState(partial: Partial<CheckoutState>): void {
    this.stateSubject.next({ ...this.stateSubject.getValue(), ...partial });
  }

  createOrder(): string | null {
    const state = this.stateSubject.getValue();
    const cartItems = this.cartService.getCartSnapshot();
    const summary = this.cartService.getSummarySnapshot();
    const userId = this.authState.getUserSnapshot()?.id;
    if (!cartItems.length) {
      return null;
    }
    const order = this.orderService.placeOrder({ state, cartItems, summary, userId });
    this.cartService.clearCart();
    this.resetState();
    return order.id;
  }

  resetState(): void {
    const email = this.authState.getUserSnapshot()?.email ?? '';
    this.stateSubject.next(this.buildDefaultState(email));
  }

  getStateSnapshot(): CheckoutState {
    return this.stateSubject.getValue();
  }

  private buildDefaultState(email = ''): CheckoutState {
    return {
      currentStep: 1,
      email,
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
      payment: {
        cardholderName: '',
        cardNumber: '',
        expMonth: '',
        expYear: '',
        cvc: '',
        saveCard: false,
      },
    };
  }

  private loadState(): CheckoutState | null {
    const stored = localStorage.getItem(this.activeStorageKey);
    if (!stored) {
      return null;
    }

    try {
      return JSON.parse(stored) as CheckoutState;
    } catch {
      return null;
    }
  }
}
