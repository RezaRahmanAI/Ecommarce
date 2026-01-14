import { Component, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { combineLatest, map, startWith } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { CartService } from '../../../../core/services/cart.service';
import { CheckoutService } from '../../../../core/services/checkout.service';
import { SHIPPING_METHODS } from '../../../../core/data/mock-shipping-methods';
import { CartItem } from '../../../../core/models/cart';
import { ShippingMethod } from '../../../../core/models/checkout';
import { PriceDisplayComponent } from '../../../../shared/components/price-display/price-display.component';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, PriceDisplayComponent],
  templateUrl: './checkout-page.component.html',
  styleUrl: './checkout-page.component.css',
})
export class CheckoutPageComponent {
  private readonly cartService = inject(CartService);
  private readonly checkoutService = inject(CheckoutService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  readonly shippingMethods = SHIPPING_METHODS;
  private readonly fallbackShippingMethod: ShippingMethod = this.shippingMethods[0] ?? {
    id: 'standard',
    label: 'Standard Shipping',
    description: '5-7 business days',
    price: 0,
    estimatedDelivery: '5-7 business days',
  };

  readonly checkoutForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    newsletter: true,
    shippingAddress: this.formBuilder.nonNullable.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      address: ['', Validators.required],
      apartment: '',
      city: ['', Validators.required],
      region: ['', Validators.required],
      postalCode: ['', Validators.required],
      country: 'United States',
    }),
    shippingMethod: this.fallbackShippingMethod.id,
    promoCode: '',
    payment: this.formBuilder.nonNullable.group({
      cardholderName: ['', Validators.required],
      cardNumber: ['', [Validators.required, Validators.minLength(12)]],
      expMonth: ['', Validators.required],
      expYear: ['', Validators.required],
      cvc: ['', Validators.required],
      saveCard: false,
    }),
  });

  currentStep = 1;

  readonly selectedShippingMethod$ = this.checkoutForm.controls.shippingMethod.valueChanges.pipe(
    startWith(this.checkoutForm.controls.shippingMethod.value ?? this.fallbackShippingMethod.id),
    map((methodId) => this.shippingMethods.find((method) => method.id === methodId) ?? this.fallbackShippingMethod),
  );

  readonly summary$ = combineLatest([this.cartService.summary$, this.selectedShippingMethod$]).pipe(
    map(([summary, method]) => ({
      ...summary,
      shipping: method.price,
      total: Number((summary.subtotal + summary.tax + method.price).toFixed(2)),
    })),
  );

  readonly vm$ = combineLatest([this.cartService.getCart(), this.summary$]).pipe(
    map(([cartItems, summary]) => ({ cartItems, summary })),
  );

  constructor() {
    this.checkoutService.state$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((state) => {
        this.currentStep = state.currentStep;
        this.checkoutForm.patchValue({
          email: state.email,
          newsletter: state.newsletter,
          shippingAddress: state.shippingAddress,
          shippingMethod: state.shippingMethodId,
          promoCode: state.promoCode,
          payment: state.payment,
        });
      });
  }

  continueToPayment(): void {
    if (this.checkoutForm.controls.email.invalid || this.checkoutForm.controls.shippingAddress.invalid) {
      this.checkoutForm.controls.email.markAsTouched();
      this.checkoutForm.controls.shippingAddress.markAllAsTouched();
      return;
    }

    this.currentStep = 2;
    this.checkoutService.setStep(this.currentStep);
    this.persistCheckoutState();
  }

  continueToReview(): void {
    if (this.checkoutForm.controls.payment.invalid) {
      this.checkoutForm.controls.payment.markAllAsTouched();
      return;
    }

    this.currentStep = 3;
    this.checkoutService.setStep(this.currentStep);
    this.persistCheckoutState();
  }

  backToShipping(): void {
    this.currentStep = 1;
    this.checkoutService.setStep(this.currentStep);
  }

  backToPayment(): void {
    this.currentStep = 2;
    this.checkoutService.setStep(this.currentStep);
  }

  placeOrder(): void {
    this.persistCheckoutState();
    const orderId = this.checkoutService.createOrder();
    if (!orderId) {
      return;
    }
    void this.router.navigate(['/order-confirmation', orderId]);
  }

  applyPromoCode(): void {
    this.persistCheckoutState();
  }

  stepClasses(step: number): string {
    if (this.currentStep === step) {
      return 'bg-primary text-white ring-4 ring-white dark:ring-background-dark';
    }
    if (this.currentStep > step) {
      return 'bg-primary text-white';
    }
    return 'bg-slate-200 dark:bg-slate-700 text-slate-600 dark:text-slate-400';
  }

  stepLabelClasses(step: number): string {
    return this.currentStep >= step ? 'text-primary dark:text-white' : 'text-slate-500 dark:text-slate-400';
  }

  trackCartItem(_: number, item: CartItem): string {
    return item.id;
  }

  private persistCheckoutState(): void {
    const paymentValue = this.checkoutForm.controls.payment.getRawValue();
    this.checkoutService.updateState({
      email: this.checkoutForm.controls.email.value ?? '',
      newsletter: this.checkoutForm.controls.newsletter.value ?? false,
      shippingAddress: this.checkoutForm.controls.shippingAddress.getRawValue(),
      shippingMethodId: this.checkoutForm.controls.shippingMethod.value ?? 'standard',
      promoCode: this.checkoutForm.controls.promoCode.value ?? '',
      payment: paymentValue,
    });
  }
}
