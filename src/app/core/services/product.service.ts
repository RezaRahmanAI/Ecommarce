import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { MOCK_PRODUCTS } from '../data/mock-products';
import { MOCK_REVIEWS } from '../data/mock-reviews';
import { Product } from '../models/product';
import { Review } from '../models/review';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly products = MOCK_PRODUCTS;

  // private readonly baseUrl = '/api/products';

  getProducts(): Observable<Product[]> {
    return of(this.products);
  }

  getByCategory(category: string): Observable<Product[]> {
    return of(this.products.filter((product) => product.category.toLowerCase() === category.toLowerCase()));
  }

  getByGender(gender: string): Observable<Product[]> {
    return of(this.products.filter((product) => product.gender === gender));
  }

  getFeatured(): Observable<Product[]> {
    return of(this.products.filter((product) => product.featured));
  }

  getNewArrivals(): Observable<Product[]> {
    return of(this.products.filter((product) => product.newArrival));
  }

  getById(id: number): Observable<Product | undefined> {
    return of(this.products.find((product) => product.id === id));
  }

  getReviewsByProductId(productId: number): Observable<Review[]> {
    return of(MOCK_REVIEWS.filter((review) => review.productId === productId));
  }
}
