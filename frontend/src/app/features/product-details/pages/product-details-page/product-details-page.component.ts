import { Component, inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import {
  BehaviorSubject,
  combineLatest,
  filter,
  map,
  switchMap,
  tap,
} from "rxjs";

import { ProductService } from "../../../../core/services/product.service";
import {
  Product,
  ProductImage,
  RatingBreakdown,
} from "../../../../core/models/product";
import { Review } from "../../../../core/models/review";
import { CartService } from "../../../../core/services/cart.service";
import { PriceDisplayComponent } from "../../../../shared/components/price-display/price-display.component";

@Component({
  selector: "app-product-details-page",
  standalone: true,
  imports: [CommonModule, RouterLink, PriceDisplayComponent],
  templateUrl: "./product-details-page.component.html",
  styleUrl: "./product-details-page.component.css",
})
export class ProductDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);
  private readonly cartService = inject(CartService);
  private readonly router = inject(Router);

  private readonly selectedColorSubject = new BehaviorSubject<string | null>(
    null,
  );
  private readonly selectedSizeSubject = new BehaviorSubject<string | null>(
    null,
  );
  private readonly quantitySubject = new BehaviorSubject<number>(1);
  private readonly selectedMediaSubject =
    new BehaviorSubject<ProductImage | null>(null);

  product$ = this.route.paramMap.pipe(
    map((params) => Number(params.get("id")) || 1),
    switchMap((id) => this.productService.getById(id)),
    filter((product): product is Product => Boolean(product)),
    tap((product) => {
      this.selectedColorSubject.next(product.variants.colors[0]?.name ?? null);
      this.selectedSizeSubject.next(product.variants.sizes[0]?.label ?? null);
      this.quantitySubject.next(1);
      this.selectedMediaSubject.next(product.images.mainImage);
    }),
  );

  reviews$ = this.route.paramMap.pipe(
    map((params) => Number(params.get("id")) || 1),
    switchMap((id) => this.productService.getReviewsByProductId(id)),
  );

  readonly vm$ = combineLatest([
    this.product$,
    this.selectedColorSubject,
    this.selectedSizeSubject,
    this.quantitySubject,
    this.selectedMediaSubject,
  ]).pipe(
    map(([product, selectedColor, selectedSize, quantity, selectedMedia]) => ({
      product,
      selectedColor,
      selectedSize,
      quantity,
      selectedMedia: this.ensureSelectedMedia(product, selectedMedia),
      gallery: this.buildGallery(product),
    })),
  );

  selectionError = "";

  fullStars(rating: number): number[] {
    return Array.from({ length: Math.floor(rating) }, (_, index) => index);
  }

  hasHalfStar(rating: number): boolean {
    return rating % 1 >= 0.5;
  }

  emptyStars(rating: number): number[] {
    const full = Math.floor(rating);
    const half = this.hasHalfStar(rating) ? 1 : 0;
    return Array.from(
      { length: Math.max(0, 5 - full - half) },
      (_, index) => index,
    );
  }

  selectedColorName(
    product: Product | null,
    selectedColor: string | null,
  ): string {
    return selectedColor ?? product?.variants.colors[0]?.name ?? "";
  }

  selectedSizeLabel(
    product: Product | null,
    selectedSize: string | null,
  ): string {
    return selectedSize ?? product?.variants.sizes[0]?.label ?? "";
  }

  selectColor(colorName: string): void {
    this.selectedColorSubject.next(colorName);
    this.selectionError = "";
  }

  selectSize(sizeLabel: string): void {
    this.selectedSizeSubject.next(sizeLabel);
    this.selectionError = "";
  }

  selectMedia(media: ProductImage): void {
    this.selectedMediaSubject.next(media);
  }

  increaseQuantity(): void {
    this.quantitySubject.next(this.quantitySubject.getValue() + 1);
  }

  decreaseQuantity(): void {
    this.quantitySubject.next(Math.max(1, this.quantitySubject.getValue() - 1));
  }

  addToCart(product: Product | null): void {
    if (!product) {
      return;
    }
    const selectedColor = this.selectedColorSubject.getValue();
    const selectedSize = this.selectedSizeSubject.getValue();
    if (!selectedColor || !selectedSize) {
      this.selectionError =
        "Please select a color and size before adding to cart.";
      return;
    }
    const quantity = this.quantitySubject.getValue();
    this.cartService.addItem(product, quantity, selectedColor, selectedSize);
    this.selectionError = "";
  }

  buyNow(product: Product | null): void {
    this.addToCart(product);
    if (!this.selectionError) {
      void this.router.navigateByUrl("/cart");
    }
  }

  trackRating(_: number, rating: RatingBreakdown): number {
    return rating.rating;
  }

  trackReview(_: number, review: Review): number {
    return review.id;
  }

  private buildGallery(product: Product): ProductImage[] {
    const gallery = [product.images.mainImage, ...product.images.thumbnails];
    const seen = new Set<string>();
    return gallery.filter((item) => {
      const key = `${item.type}-${item.url}`;
      if (seen.has(key)) {
        return false;
      }
      seen.add(key);
      return true;
    });
  }

  private ensureSelectedMedia(
    product: Product,
    selectedMedia: ProductImage | null,
  ): ProductImage {
    return selectedMedia ?? product.images.mainImage;
  }
}
