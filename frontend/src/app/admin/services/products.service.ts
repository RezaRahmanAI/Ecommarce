import { Injectable, inject } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, map, of } from 'rxjs';

import { Product as StoreProduct } from '../../core/models/product';
import {
  Product,
  ProductCreatePayload,
  ProductUpdatePayload,
  ProductsQueryParams,
} from '../models/products.models';
import { ApiHttpClient } from '../../core/http/http-client';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly api = inject(ApiHttpClient);
  private readonly catalogSubject = new BehaviorSubject<Product[]>([]);
  private catalogLoaded = false;
  private catalogLoading = false;

  constructor() {
    this.loadCatalog();
  }

  getCatalogProducts(): Observable<Product[]> {
    if (!this.catalogLoaded && !this.catalogLoading) {
      this.loadCatalog();
    }
    return this.catalogSubject.asObservable();
  }

  getCatalogSnapshot(): Product[] {
    return [...this.catalogSubject.getValue()];
  }

  getProducts(params: ProductsQueryParams): Observable<{ items: Product[]; total: number }> {
    const queryParams = new HttpParams({
      fromObject: {
        searchTerm: params.searchTerm,
        category: params.category,
        statusTab: params.statusTab,
        page: params.page,
        pageSize: params.pageSize,
      },
    });
    return this.api.get<{ items: Product[]; total: number }>('/admin/products', {
      params: queryParams,
    });
  }

  getFilteredProducts(params: ProductsQueryParams): Observable<Product[]> {
    const queryParams = new HttpParams({
      fromObject: {
        searchTerm: params.searchTerm,
        category: params.category,
        statusTab: params.statusTab,
      },
    });
    return this.api.get<Product[]>('/admin/products/filtered', { params: queryParams });
  }

  exportProducts(params: ProductsQueryParams): Observable<string> {
    return this.getFilteredProducts(params).pipe(map((rows) => this.buildCsv(rows)));
  }

  deleteProduct(productId: number): Observable<boolean> {
    return this.api.delete<boolean>(`/admin/products/${productId}`).pipe(
      map((success) => {
        if (success) {
          this.updateCatalogSnapshot((products) =>
            products.filter((product) => product.id !== productId),
          );
        }
        return success;
      }),
    );
  }

  createProduct(payload: ProductCreatePayload): Observable<Product> {
    return this.api.post<Product>('/admin/products', payload).pipe(
      map((created) => {
        this.updateCatalogSnapshot((products) => [created, ...products]);
        return created;
      }),
    );
  }

  getProductById(productId: number): Observable<Product> {
    return this.api.get<Product>(`/admin/products/${productId}`);
  }

  updateProduct(productId: number, payload: ProductUpdatePayload): Observable<Product> {
    return this.api.put<Product>(`/admin/products/${productId}`, payload).pipe(
      map((updated) => {
        this.updateCatalogSnapshot((products) => {
          const index = products.findIndex((item) => item.id === productId);
          if (index === -1) {
            return [updated, ...products];
          }
          return [
            ...products.slice(0, index),
            updated,
            ...products.slice(index + 1),
          ];
        });
        return updated;
      }),
    );
  }

  uploadProductMedia(files: File[]): Observable<string[]> {
    if (files.length === 0) {
      return of([]);
    }
    const formData = new FormData();
    files.forEach((file) => formData.append('files', file));
    return this.api.post<string[]>('/admin/products/media', formData);
  }

  removeProductMedia(productId: number, mediaUrl: string): Observable<boolean> {
    return this.api.post<boolean>(`/admin/products/${productId}/media/remove`, { mediaUrl }).pipe(
      map((success) => {
        if (success) {
          this.updateCatalogSnapshot((products) =>
            products.map((product) =>
              product.id === productId
                ? this.removeMediaFromProduct(product, mediaUrl)
                : product,
            ),
          );
        }
        return success;
      }),
    );
  }

  private loadCatalog(): void {
    this.catalogLoading = true;
    this.api.get<Product[]>('/admin/products/catalog').subscribe({
      next: (products) => {
        this.catalogLoaded = true;
        this.catalogSubject.next(products);
      },
      error: () => {
        this.catalogLoading = false;
      },
      complete: () => {
        this.catalogLoading = false;
      },
    });
  }

  private updateCatalogSnapshot(updater: (products: Product[]) => Product[]): void {
    const current = this.catalogSubject.getValue();
    const next = updater(current);
    this.catalogLoaded = true;
    this.catalogSubject.next(next);
  }

  private buildCsv(rows: Product[]): string {
    const header = [
      'ID',
      'Name',
      'Category',
      'SKU',
      'Stock',
      'Price',
      'Purchase Rate',
      'Status',
      'Tags',
    ];
    const csvRows = rows.map((product) => [
      product.id,
      product.name,
      product.category,
      product.sku,
      String(product.stock),
      product.price.toFixed(2),
      (product.purchaseRate ?? 0).toFixed(2),
      product.status,
      (product.tags ?? []).join('|'),
    ]);

    return [header, ...csvRows]
      .map((row) => row.map((value) => `"${String(value).replace(/"/g, '""')}"`).join(','))
      .join('\n');
  }

  private removeMediaFromProduct(product: Product, mediaUrl: string): Product {
    const updatedMedia = (product.mediaUrls ?? []).filter((url) => url !== mediaUrl);
    const images = this.buildImagesFromMedia(updatedMedia, product.name, product.images);
    const imageUrl = product.imageUrl === mediaUrl ? updatedMedia[0] : product.imageUrl;

    return {
      ...product,
      mediaUrls: updatedMedia,
      images,
      imageUrl,
    };
  }

  private buildImagesFromMedia(
    mediaUrls: string[],
    name: string,
    existing?: StoreProduct['images'],
  ): StoreProduct['images'] {
    const media = (mediaUrls.length ? mediaUrls : [existing?.mainImage.url ?? ''])
      .filter(Boolean) as string[];
    const [mainUrl, ...thumbnails] = media.length ? media : [''];
    const mainImage = {
      type: (existing?.mainImage.type ?? 'image') as 'image' | 'video',
      label: existing?.mainImage.label ?? 'Main',
      url: mainUrl,
      alt: existing?.mainImage.alt ?? `${name} image`,
    };
    const thumbnailUrls = thumbnails.length ? thumbnails : [mainUrl];
    const thumbnailImages = thumbnailUrls.map((url, index) => ({
      type: 'image' as const,
      label: existing?.thumbnails[index]?.label ?? `Gallery ${index + 1}`,
      url,
      alt: existing?.thumbnails[index]?.alt ?? `${name} gallery ${index + 1}`,
    }));

    return {
      mainImage,
      thumbnails: thumbnailImages,
    };
  }
}
