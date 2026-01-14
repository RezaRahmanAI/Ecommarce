import { Injectable } from '@angular/core';
import { Observable, of, map } from 'rxjs';

import { ProductsService } from '../../admin/services/products.service';
import { MOCK_REVIEWS } from '../data/mock-reviews';
import { Product } from '../models/product';
import { Review } from '../models/review';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private readonly productsService: ProductsService) {}

  // private readonly baseUrl = '/api/products';

  getProducts(): Observable<Product[]> {
    return this.productsService.getCatalogProducts();
  }

  getByCategory(category: string): Observable<Product[]> {
    return this.productsService.getCatalogProducts().pipe(
      map((products) =>
        products.filter((product) => product.category.toLowerCase() === category.toLowerCase()),
      ),
    );
  }

  getByGender(gender: string): Observable<Product[]> {
    return this.productsService
      .getCatalogProducts()
      .pipe(map((products) => products.filter((product) => product.gender === gender)));
  }

  getFeatured(): Observable<Product[]> {
    return this.productsService
      .getCatalogProducts()
      .pipe(map((products) => products.filter((product) => product.featured)));
  }

  getNewArrivals(): Observable<Product[]> {
    return this.productsService
      .getCatalogProducts()
      .pipe(map((products) => products.filter((product) => product.newArrival)));
  }

  getById(id: number): Observable<Product | undefined> {
    return this.productsService
      .getCatalogProducts()
      .pipe(map((products) => products.find((product) => product.id === id)));
  }

  getReviewsByProductId(productId: number): Observable<Review[]> {
    return of(MOCK_REVIEWS.filter((review) => review.productId === productId));
  }

  getRecommendedProducts(): Observable<Product[]> {
    return this.productsService.getCatalogProducts().pipe(
      map((products) => {
        const highlighted = products.filter((product) => product.featured || product.newArrival);
        return (highlighted.length ? highlighted : products).slice(0, 4);
      }),
    );
  }
}
