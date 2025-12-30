import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { combineLatest, map, startWith } from 'rxjs';

import { CartService } from '../../../../core/services/cart.service';
import { CheckoutService } from '../../../../core/services/checkout.service';
import { SHIPPING_METHODS } from '../../../../core/data/mock-shipping-methods';
import { CartItem } from '../../../../core/models/cart';
import { ShippingMethod } from '../../../../core/models/checkout';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './checkout-page.component.html',
  styleUrl: './checkout-page.component.css',
})
export class CheckoutPageComponent {
  private readonly cartService = inject(CartService);
  private readonly checkoutService = inject(CheckoutService);
  private readonly formBuilder = inject(FormBuilder);

  readonly shippingMethods = SHIPPING_METHODS;
  private readonly fallbackShippingMethod: ShippingMethod = this.shippingMethods[0] ?? {
    id: 'standard',
    label: 'Standard Shipping',
    description: '5-7 business days',
    price: 0,
    estimatedDelivery: '5-7 business days',
  };

  readonly checkoutForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    newsletter: [true],
    shippingAddress: this.formBuilder.group({
      firstName: [''],
      lastName: [''],
      address: [''],
      apartment: [''],
      city: [''],
      region: [''],
      postalCode: [''],
      country: ['United States'],
    }),
    shippingMethod: [this.fallbackShippingMethod.id],
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

  continueToPayment(): void {
    this.currentStep = 2;
    this.checkoutService.setStep(this.currentStep);
    this.checkoutService.updateState({
      email: this.checkoutForm.controls.email.value ?? '',
      newsletter: this.checkoutForm.controls.newsletter.value ?? false,
      shippingAddress: this.checkoutForm.controls.shippingAddress.getRawValue(),
      shippingMethodId: this.checkoutForm.controls.shippingMethod.value ?? 'standard',
    });
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

}
