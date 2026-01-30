import { Injectable, inject } from "@angular/core";
import { Observable, of } from "rxjs";

import { ApiHttpClient } from "../http/http-client";
import { MOCK_REVIEWS } from "../data/mock-reviews";
import { Product } from "../models/product";
import { Review } from "../models/review";

@Injectable({
  providedIn: "root",
})
export class ProductService {
  private readonly api = inject(ApiHttpClient);
  private readonly baseUrl = "/products";

  getProducts(): Observable<Product[]> {
    return this.api.get<Product[]>(this.baseUrl);
  }

  getByCategory(category: string): Observable<Product[]> {
    return this.api.get<Product[]>(`${this.baseUrl}`, {
      params: { category } as any,
    });
  }

  getByGender(gender: string): Observable<Product[]> {
    return this.api.get<Product[]>(`${this.baseUrl}`, {
      params: { gender } as any,
    });
  }

  getFeatured(): Observable<Product[]> {
    return this.api.get<Product[]>(`${this.baseUrl}/featured`);
  }

  getNewArrivals(): Observable<Product[]> {
    return this.api.get<Product[]>(`${this.baseUrl}/new-arrivals`);
  }

  getById(id: number): Observable<Product | undefined> {
    return this.api.get<Product>(`${this.baseUrl}/${id}`);
  }

  getReviewsByProductId(productId: number): Observable<Review[]> {
    return of(MOCK_REVIEWS.filter((review) => review.productId === productId));
  }

  getRecommendedProducts(): Observable<Product[]> {
    return this.getFeatured();
  }
}
