import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { MOCK_PRODUCTS } from '../../core/data/mock-products';
import { Product as StoreProduct } from '../../core/models/product';
import {
  Product,
  ProductCreatePayload,
  ProductUpdatePayload,
  ProductVariantEdit,
  ProductsQueryParams,
  ProductsStatusTab,
} from '../models/products.models';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private products: Product[] = this.seedProducts(MOCK_PRODUCTS);

  getCatalogProducts(): Observable<Product[]> {
    return of(this.products);
  }

  getCatalogSnapshot(): Product[] {
    return [...this.products];
  }

  getProducts(params: ProductsQueryParams): Observable<{ items: Product[]; total: number }> {
    const filtered = this.filterProducts(params);
    const total = filtered.length;
    const startIndex = (params.page - 1) * params.pageSize;
    const items = filtered.slice(startIndex, startIndex + params.pageSize);
    return of({ items, total });
  }

  exportProducts(params: ProductsQueryParams): string {
    const rows = this.filterProducts(params);
    const header = ['ID', 'Name', 'Category', 'SKU', 'Stock', 'Price', 'Status', 'Tags'];
    const csvRows = rows.map((product) => [
      product.id,
      product.name,
      product.category,
      product.sku,
      String(product.stock),
      product.price.toFixed(2),
      product.status,
      (product.tags ?? []).join('|'),
    ]);

    return [header, ...csvRows]
      .map((row) => row.map((value) => `"${String(value).replace(/"/g, '""')}"`).join(','))
      .join('\n');
  }

  deleteProduct(productId: number): void {
    this.products = this.products.filter((product) => product.id !== productId);
  }

  createProduct(payload: ProductCreatePayload): Observable<Product> {
    const nextId = this.nextProductId();
    const mediaUrls = [payload.media.mainImage.url, ...payload.media.thumbnails.map((item) => item.url)].filter(
      Boolean,
    );
    const inventoryVariants = this.buildInventoryVariantsFromPayload(payload, nextId, mediaUrls[0]);
    const stock = inventoryVariants.reduce((sum, variant) => sum + (variant.inventory ?? 0), 0);
    const status = this.resolveStatus(payload.statusActive, stock);
    const newProduct: Product = {
      id: nextId,
      name: payload.name,
      description: payload.description,
      category: payload.category,
      subCategory: payload.subCategory,
      tags: payload.tags,
      badges: payload.badges,
      price: payload.salePrice ?? payload.price,
      salePrice: payload.salePrice,
      gender: payload.gender,
      ratings: payload.ratings,
      images: payload.media,
      variants: payload.variants,
      meta: payload.meta,
      relatedProducts: [],
      featured: payload.featured,
      newArrival: payload.newArrival,
      sku: this.formatSku(nextId),
      stock,
      status,
      imageUrl: mediaUrls[0],
      statusActive: payload.statusActive,
      mediaUrls,
      basePrice: payload.price,
      inventoryVariants,
    };
    this.products = [newProduct, ...this.products];
    return of(newProduct);
  }

  getProductById(productId: number): Observable<Product> {
    const product = this.products.find((item) => item.id === productId) ?? this.products[0];

    if (!product) {
      return of(this.buildEmptyProduct(productId));
    }

    return of(this.buildProductDetail(product));
  }

  updateProduct(productId: number, payload: ProductUpdatePayload): Observable<Product> {
    const existingIndex = this.products.findIndex((item) => item.id === productId);
    const existing = this.products[existingIndex];
    const inventoryVariants = payload.inventoryVariants.length
      ? payload.inventoryVariants
      : existing?.inventoryVariants ?? [];
    const stock = inventoryVariants.reduce((sum, variant) => sum + (variant.inventory ?? 0), 0);
    const status = this.resolveStatus(payload.statusActive, stock);
    const mediaUrls = payload.mediaUrls.length ? payload.mediaUrls : existing?.mediaUrls ?? [];
    const images = this.buildImagesFromMedia(mediaUrls, payload.name, existing?.images);
    const updatedProduct: Product = {
      ...(existing ?? this.buildEmptyProduct(productId)),
      id: productId,
      name: payload.name,
      description: payload.description,
      category: payload.category,
      subCategory: payload.subCategory ?? '',
      tags: payload.tags,
      badges: payload.badges,
      gender: payload.gender,
      featured: payload.featured,
      newArrival: payload.newArrival,
      basePrice: payload.basePrice,
      salePrice: payload.salePrice,
      price: payload.salePrice ?? payload.basePrice,
      sku: inventoryVariants[0]?.sku ?? existing?.sku ?? this.formatSku(productId),
      stock,
      status,
      statusActive: payload.statusActive,
      mediaUrls,
      imageUrl: mediaUrls[0],
      inventoryVariants,
      images,
    };

    if (existingIndex >= 0) {
      this.products = [
        ...this.products.slice(0, existingIndex),
        updatedProduct,
        ...this.products.slice(existingIndex + 1),
      ];
    } else {
      this.products = [updatedProduct, ...this.products];
    }

    return of(updatedProduct);
  }

  uploadProductMedia(files: File[]): Observable<string[]> {
    if (files.length === 0) {
      return of([]);
    }
    const urls = files.map((file) => URL.createObjectURL(file));
    return of(urls);
  }

  removeProductMedia(productId: number, mediaUrl: string): Observable<boolean> {
    const product = this.products.find((item) => item.id === productId);
    if (!product) {
      return of(false);
    }
    const updatedMedia = (product.mediaUrls ?? []).filter((url) => url !== mediaUrl);
    product.mediaUrls = updatedMedia;
    product.images = this.buildImagesFromMedia(updatedMedia, product.name, product.images);
    if (product.imageUrl === mediaUrl) {
      product.imageUrl = updatedMedia[0];
    }
    return of(true);
  }

  private seedProducts(products: StoreProduct[]): Product[] {
    return products.map((product) => {
      const inventoryVariants = this.buildInventoryVariants(product);
      const stock = inventoryVariants.reduce((sum, variant) => sum + (variant.inventory ?? 0), 0);
      const status = this.resolveStatus(true, stock);
      const mediaUrls = [product.images.mainImage.url, ...product.images.thumbnails.map((item) => item.url)].filter(
        Boolean,
      );

      const gender = (product.gender ?? 'women') as Product['gender'];

      return {
        ...product,
        gender,
        badges: product.badges ?? [],
        tags: product.tags ?? [],
        sku: this.formatSku(product.id),
        stock,
        status,
        imageUrl: product.images.mainImage.url,
        statusActive: status === 'Active',
        mediaUrls,
        basePrice: product.price,
        inventoryVariants,
      };
    });
  }

  private buildInventoryVariants(product: StoreProduct): ProductVariantEdit[] {
    const baseSku = this.formatSku(product.id);
    const price = product.salePrice ?? product.price;
    const mainImage = product.images.mainImage.url;
    const primaryColor = product.variants.colors[0]?.name ?? 'Default';
    const sizes = product.variants.sizes.length ? product.variants.sizes : [{ label: 'One Size', stock: 0, selected: true }];

    return sizes.map((size, index) => ({
      label: `${size.label} / ${primaryColor}`,
      price,
      sku: `${baseSku}-${index + 1}`,
      inventory: Number(size.stock ?? 0),
      imageUrl: mainImage,
    }));
  }

  private buildInventoryVariantsFromPayload(
    payload: ProductCreatePayload,
    productId: number,
    imageUrl?: string,
  ): ProductVariantEdit[] {
    const baseSku = this.formatSku(productId);
    const price = payload.salePrice ?? payload.price;
    const primaryColor = payload.variants.colors[0]?.name ?? 'Default';
    const sizes = payload.variants.sizes.length
      ? payload.variants.sizes
      : [{ label: 'One Size', stock: 0, selected: true }];

    return sizes.map((size, index) => ({
      label: `${size.label} / ${primaryColor}`,
      price,
      sku: `${baseSku}-${index + 1}`,
      inventory: Number(size.stock ?? 0),
      imageUrl,
    }));
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

  private buildEmptyProduct(productId: number): Product {
    return {
      id: productId,
      name: 'New Product',
      description: '',
      category: 'Women',
      subCategory: '',
      tags: [],
      badges: [],
      price: 0,
      salePrice: undefined,
      gender: 'women',
      ratings: {
        avgRating: 0,
        reviewCount: 0,
        ratingBreakdown: [
          { rating: 5, percentage: 0 },
          { rating: 4, percentage: 0 },
          { rating: 3, percentage: 0 },
          { rating: 2, percentage: 0 },
          { rating: 1, percentage: 0 },
        ],
      },
      images: {
        mainImage: {
          type: 'image',
          label: 'Main',
          url: 'https://via.placeholder.com/600x800?text=Product+Image',
          alt: 'Product image',
        },
        thumbnails: [
          {
            type: 'image',
            label: 'Gallery',
            url: 'https://via.placeholder.com/600x800?text=Product+Image',
            alt: 'Product image',
          },
        ],
      },
      variants: {
        colors: [{ name: 'Default', hex: '#111827', selected: true }],
        sizes: [{ label: 'One Size', stock: 0, selected: true }],
      },
      meta: {
        fabricAndCare: '',
        shippingAndReturns: '',
      },
      relatedProducts: [],
      featured: false,
      newArrival: false,
      sku: this.formatSku(productId),
      stock: 0,
      status: 'Draft',
      imageUrl: '',
      statusActive: false,
      mediaUrls: [],
      basePrice: 0,
      inventoryVariants: [],
    };
  }

  private resolveStatus(statusActive: boolean, stock: number): Product['status'] {
    if (!statusActive) {
      return 'Draft';
    }
    if (stock === 0) {
      return 'Out of Stock';
    }
    return 'Active';
  }

  private formatSku(productId: number): string {
    return `SKU-${String(productId).padStart(5, '0')}`;
  }

  private nextProductId(): number {
    const ids = this.products.map((product) => product.id);
    return ids.length ? Math.max(...ids) + 1 : 1;
  }

  private filterProducts(params: ProductsQueryParams): Product[] {
    const normalizedSearch = params.searchTerm.trim().toLowerCase();

    return this.products.filter((product) => {
      const matchesSearch = normalizedSearch
        ? this.matchesSearch(product, normalizedSearch)
        : true;
      const matchesCategory =
        params.category === 'All Categories' || product.category === params.category;
      const matchesStatus = this.matchesStatus(product, params.statusTab);

      return matchesSearch && matchesCategory && matchesStatus;
    });
  }

  private matchesSearch(product: Product, searchTerm: string): boolean {
    const haystack = [product.name, product.sku, ...(product.tags ?? [])]
      .join(' ')
      .toLowerCase();
    return haystack.includes(searchTerm);
  }

  private matchesStatus(product: Product, statusTab: ProductsStatusTab): boolean {
    switch (statusTab) {
      case 'Active':
        return product.status === 'Active';
      case 'Drafts':
        return product.status === 'Draft';
      case 'Archived':
        return product.status === 'Archived';
      default:
        return true;
    }
  }

  private buildProductDetail(product: Product): Product {
    const basePrice = product.basePrice ?? product.price ?? 0;
    const salePrice = product.salePrice ?? undefined;
    const mediaUrls = product.mediaUrls ?? (product.imageUrl ? [product.imageUrl] : []);
    const inventoryVariants =
      product.inventoryVariants?.length && product.inventoryVariants.length > 0
        ? product.inventoryVariants
        : this.buildInventoryVariants(product);

    return {
      ...product,
      description:
        product.description ??
        'Elegant chiffon abaya perfect for special occasions. Features delicate embroidery on the sleeves and hemline. Includes matching shayla.',
      subCategory: product.subCategory ?? 'Occasion Wear',
      basePrice,
      salePrice,
      statusActive: product.statusActive ?? product.status === 'Active',
      mediaUrls,
      inventoryVariants,
    };
  }
}
