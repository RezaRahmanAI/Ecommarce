import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';

import { ProductService } from '../../../../core/services/product.service';
import { Product, RatingBreakdown } from '../../../../core/models/product';
import { Review } from '../../../../core/models/review';

@Component({
  selector: 'app-product-details-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-details-page.component.html',
  styleUrl: './product-details-page.component.css',
})
export class ProductDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);

  product$ = this.route.paramMap.pipe(
    map((params) => Number(params.get('id')) || 1),
    switchMap((id) => this.productService.getById(id)),
  );

  reviews$ = this.route.paramMap.pipe(
    map((params) => Number(params.get('id')) || 1),
    switchMap((id) => this.productService.getReviewsByProductId(id)),
  );

  fullStars(rating: number): number[] {
    return Array.from({ length: Math.floor(rating) }, (_, index) => index);
  }

  hasHalfStar(rating: number): boolean {
    return rating % 1 >= 0.5;
  }

  emptyStars(rating: number): number[] {
    const full = Math.floor(rating);
    const half = this.hasHalfStar(rating) ? 1 : 0;
    return Array.from({ length: Math.max(0, 5 - full - half) }, (_, index) => index);
  }

  selectedColorName(product: Product): string {
    return product.variants.colors.find((color) => color.selected)?.name ?? '';
  }

  selectedSizeLabel(product: Product): string {
    return product.variants.sizes.find((size) => size.selected)?.label ?? '';
  }

  trackRating(_: number, rating: RatingBreakdown): number {
    return rating.rating;
  }

  trackReview(_: number, review: Review): number {
    return review.id;
  }
}
