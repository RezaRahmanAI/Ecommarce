import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';

import { CartItem, CartSummary } from '../models/cart';
import { MOCK_PRODUCTS } from '../data/mock-products';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly freeShippingThreshold = 150;
  private readonly taxRate = 0.08;

  private readonly cartItemsSubject = new BehaviorSubject<CartItem[]>(this.buildInitialCart());
  readonly cartItems$ = this.cartItemsSubject.asObservable();

  readonly summary$ = this.cartItems$.pipe(map((items) => this.calculateSummary(items)));

  getCart(): Observable<CartItem[]> {
    return this.cartItems$;
  }

  getCartSnapshot(): CartItem[] {
    return this.cartItemsSubject.getValue();
  }

  getSummarySnapshot(): CartSummary {
    return this.calculateSummary(this.cartItemsSubject.getValue());
  }

  addItem(productId: number, quantity = 1, color?: string, size?: string): void {
    const product = MOCK_PRODUCTS.find((item) => item.id === productId);
    if (!product) {
      return;
    }

    const items = this.cartItemsSubject.getValue();
    const existing = items.find(
      (item) => item.productId === productId && item.color === (color ?? item.color) && item.size === (size ?? item.size),
    );

    if (existing) {
      this.updateQty(existing.id, existing.quantity + quantity);
      return;
    }

    const nextItems = [...items, this.createCartItem(product, quantity, color, size)];
    this.cartItemsSubject.next(nextItems);
  }

  removeItem(cartItemId: string): void {
    this.cartItemsSubject.next(this.cartItemsSubject.getValue().filter((item) => item.id !== cartItemId));
  }

  updateQty(cartItemId: string, quantity: number): void {
    const sanitizedQty = Math.max(0, quantity);
    const nextItems = this.cartItemsSubject
      .getValue()
      .map((item) => (item.id === cartItemId ? { ...item, quantity: sanitizedQty } : item))
      .filter((item) => item.quantity > 0);
    this.cartItemsSubject.next(nextItems);
  }

  private buildInitialCart(): CartItem[] {
    const [first, second] = MOCK_PRODUCTS;
    if (!first || !second) {
      return [];
    }

    return [
      this.createCartItem(first, 1, first.variants.colors[0]?.name, first.variants.sizes[0]?.label),
      this.createCartItem(second, 2, second.variants.colors[0]?.name, second.variants.sizes[0]?.label),
    ];
  }

  private createCartItem(product: Product, quantity: number, color?: string, size?: string): CartItem {
    const resolvedColor = color ?? product.variants.colors.find((item) => item.selected)?.name ?? 'Default';
    const resolvedSize = size ?? product.variants.sizes.find((item) => item.selected)?.label ?? 'One Size';

    return {
      id: `cart-${product.id}-${resolvedColor}-${resolvedSize}-${Math.random().toString(36).slice(2, 8)}`,
      productId: product.id,
      name: product.name,
      price: product.salePrice ?? product.price,
      quantity,
      color: resolvedColor,
      size: resolvedSize,
      imageUrl: product.images.mainImage.url,
      imageAlt: product.images.mainImage.alt,
    };
  }

  private calculateSummary(items: CartItem[]): CartSummary {
    const subtotal = items.reduce((total, item) => total + item.price * item.quantity, 0);
    const tax = Number((subtotal * this.taxRate).toFixed(2));
    const shipping = subtotal >= this.freeShippingThreshold ? 0 : 12;
    const total = Number((subtotal + tax + shipping).toFixed(2));
    const freeShippingRemaining = Math.max(this.freeShippingThreshold - subtotal, 0);
    const freeShippingProgress = Math.min((subtotal / this.freeShippingThreshold) * 100, 100);

    return {
      itemsCount: items.length,
      subtotal,
      tax,
      shipping,
      total,
      freeShippingThreshold: this.freeShippingThreshold,
      freeShippingRemaining,
      freeShippingProgress,
    };
  }
}
