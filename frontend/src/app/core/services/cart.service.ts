import { Injectable, inject } from "@angular/core";
import { BehaviorSubject, Observable, map } from "rxjs";

import { CartItem, CartSummary } from "../models/cart";
import { Product } from "../models/product";

@Injectable({
  providedIn: "root",
})
export class CartService {
  private readonly freeShippingThreshold = 5000;
  private readonly taxRate = 0.08;
  private readonly storageKey = "cart_items";

  private readonly cartItemsSubject = new BehaviorSubject<CartItem[]>(
    this.loadCart(),
  );
  readonly cartItems$ = this.cartItemsSubject.asObservable();

  readonly summary$ = this.cartItems$.pipe(
    map((items) => this.calculateSummary(items)),
  );

  constructor() {
    this.cartItems$.subscribe((items) => {
      localStorage.setItem(this.storageKey, JSON.stringify(items));
    });
  }

  getCart(): Observable<CartItem[]> {
    return this.cartItems$;
  }

  getCartSnapshot(): CartItem[] {
    return this.cartItemsSubject.getValue();
  }

  getSummarySnapshot(): CartSummary {
    return this.calculateSummary(this.cartItemsSubject.getValue());
  }

  addItem(product: Product, quantity = 1, color?: string, size?: string): void {
    const resolvedColor =
      color ?? product.variants.colors[0]?.name ?? "Default";
    const resolvedSize = size ?? product.variants.sizes[0]?.label ?? "One Size";
    const items = this.cartItemsSubject.getValue();
    const existing = items.find(
      (item) =>
        item.productId === product.id &&
        item.color === resolvedColor &&
        item.size === resolvedSize,
    );

    if (existing) {
      this.updateQty(existing.id, existing.quantity + quantity);
      return;
    }

    const nextItems = [
      ...items,
      this.createCartItem(product, quantity, resolvedColor, resolvedSize),
    ];
    this.cartItemsSubject.next(nextItems);
  }

  removeItem(cartItemId: string): void {
    this.cartItemsSubject.next(
      this.cartItemsSubject.getValue().filter((item) => item.id !== cartItemId),
    );
  }

  updateQty(cartItemId: string, quantity: number): void {
    const sanitizedQty = Math.max(0, quantity);
    const nextItems = this.cartItemsSubject
      .getValue()
      .map((item) =>
        item.id === cartItemId ? { ...item, quantity: sanitizedQty } : item,
      )
      .filter((item) => item.quantity > 0);
    this.cartItemsSubject.next(nextItems);
  }

  clearCart(): void {
    this.cartItemsSubject.next([]);
  }

  private createCartItem(
    product: Product,
    quantity: number,
    color?: string,
    size?: string,
  ): CartItem {
    const resolvedColor =
      color ?? product.variants.colors[0]?.name ?? "Default";
    const resolvedSize = size ?? product.variants.sizes[0]?.label ?? "One Size";

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
    const subtotal = items.reduce(
      (total, item) => total + item.price * item.quantity,
      0,
    );
    const tax = Number((subtotal * this.taxRate).toFixed(2));
    const shipping = subtotal >= this.freeShippingThreshold ? 0 : 120;
    const total = Number((subtotal + tax + shipping).toFixed(2));
    const freeShippingRemaining = Math.max(
      this.freeShippingThreshold - subtotal,
      0,
    );
    const freeShippingProgress = Math.min(
      (subtotal / this.freeShippingThreshold) * 100,
      100,
    );
    const itemsCount = items.reduce((total, item) => total + item.quantity, 0);

    return {
      itemsCount,
      subtotal,
      tax,
      shipping,
      total,
      freeShippingThreshold: this.freeShippingThreshold,
      freeShippingRemaining,
      freeShippingProgress,
    };
  }

  private loadCart(): CartItem[] {
    const stored = localStorage.getItem(this.storageKey);
    if (!stored) {
      return [];
    }

    try {
      return JSON.parse(stored) as CartItem[];
    } catch {
      return [];
    }
  }
}
