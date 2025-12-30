import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { MOCK_PRODUCTS } from '../data/mock-products';
import { Product } from '../models/product';

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
}
