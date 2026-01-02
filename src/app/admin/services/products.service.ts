import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import {
  Product,
  ProductCreatePayload,
  ProductUpdatePayload,
  ProductsQueryParams,
  ProductsStatusTab,
} from '../models/products.models';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private products: Product[] = [
    {
      id: 'prod-001',
      name: 'Silk Chiffon Hijab',
      category: 'Hijabs',
      sku: 'SKU-00128',
      stock: 120,
      price: 25,
      status: 'Active',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuCgFm0GafajhfcXXJq_zACIWQOYFkdpjyWx6RuLIuYz5EMwhTEbEFKFosSW6L5dVxFIaT8p0fk5aW3d9nFEUS3iouxGJePQLEb3Q9rGlWpHgMj5PtYZmVNmIyrE-nC-FDZJt-Y_3BhibVcWlqbMOyq36J7XO7cEtudTKFyy9YYbCH9pjeBSRmNiWBL1p0or-yJpHVNrAt8P11XEdSIZHBWRyyk9PYVjQu1sSe1TJDuUwlLwuCu7prD-YUcjVQKwC-TbHB-VFQiP0ZQ',
      tags: ['silk', 'chiffon', 'best-seller'],
    },
    {
      id: 'prod-002',
      name: 'Classic Black Abaya',
      category: 'Abayas',
      sku: 'SKU-00342',
      stock: 4,
      price: 85,
      status: 'Active',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuCJUBnr70x_k80CcNgeNHOwkExnoXbYsndKDdlpvRbdAg7ssFcDw5ofMLQ-uT56MgZC78K4XsYXR8y9VYVzlR1Rx-toA3S4IBkCbRCVWyTBvpzAAOwB7jOYSxIfe7-OOANuu8w2CuGlOA2Z0u2gLylMUMqnfkCM1A_suXLGmCRKN9F-689s4seXLj1c6QYoMjpR9UokFJ0BxHeVzva6YStUXriYqyN_g7ZVuXMnAGu8IK7fmHoFBW1sWLxOgJBE8kHy8oaZr6uxV4Q',
      tags: ['abaya', 'classic'],
    },
    {
      id: 'prod-003',
      name: 'Floral Maxi Dress',
      category: 'Dresses',
      sku: 'SKU-00991',
      stock: 56,
      price: 110,
      status: 'Draft',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuBP8A-FdsFisbmLV3S5AzRCR1i6Xumb-SY9kU2H52Xd6UOIRWy73WTxYClYPs2Lxc7J0hdg6BVjYrRIIVnoxX0mtPKJfrQ4SlUcCFvf03nDASQd4b02XZdgWq2h_hvt-tbfPoYfKQNMYjzsAklwaBGAfSdIs_C8u9bJsp4XnfmsDnEMvbZoYXuMklzSTV6CXdFfoIQWf9UHiTDONwdIz6a0Y3XRMpJFVZN-HLLegJxe3jG5jUs8lg3s8xEbijj5lHh2tpwnMqMSMiw',
      tags: ['floral', 'maxi'],
    },
    {
      id: 'prod-004',
      name: 'Cotton Prayer Set',
      category: 'Prayer Sets',
      sku: 'SKU-00404',
      stock: 0,
      price: 40,
      status: 'Out of Stock',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuAomXm685xW4XZ7BYSgza88FT8TQ-ApNL5rydy-RhiggIhlyZHlA3UEY5DN2JcLIbyeZw5EhGubmPcvgRPhyU-6fjvk0RfMmSfgGhd8Y_dZuD5cz4Vmrp-kxxQ-nqYfVWAo_cQ0n8aUZSAQbJxR6-c1HsnEGFyKeDfsxgBfdSdp9P142otZMTqhFMGE7CklwxWbwU-wYo7QxmEghMZm6NDe9wMidfPCh-YA8KvEhRDXHF5kPL12frclEdfwDkFM_ChYnWK-EPnqkoA',
      tags: ['prayer', 'cotton'],
    },
    {
      id: 'prod-005',
      name: 'Linen Open Abaya',
      category: 'Abayas',
      sku: 'SKU-01110',
      stock: 18,
      price: 98,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=200&q=80',
      tags: ['linen', 'open'],
    },
    {
      id: 'prod-006',
      name: 'Everyday Jersey Hijab',
      category: 'Hijabs',
      sku: 'SKU-01111',
      stock: 72,
      price: 19,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['jersey', 'everyday'],
    },
    {
      id: 'prod-007',
      name: 'Embroidered Prayer Set',
      category: 'Prayer Sets',
      sku: 'SKU-01112',
      stock: 8,
      price: 65,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['embroidered', 'prayer'],
    },
    {
      id: 'prod-008',
      name: 'Velvet Modest Dress',
      category: 'Modest Dresses',
      sku: 'SKU-01113',
      stock: 2,
      price: 145,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=200&q=80',
      tags: ['velvet', 'evening'],
    },
    {
      id: 'prod-009',
      name: 'Pleated Modest Dress',
      category: 'Modest Dresses',
      sku: 'SKU-01114',
      stock: 0,
      price: 120,
      status: 'Out of Stock',
      imageUrl:
        'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?auto=format&fit=crop&w=200&q=80',
      tags: ['pleated'],
    },
    {
      id: 'prod-010',
      name: 'Soft Satin Abaya',
      category: 'Abayas',
      sku: 'SKU-01115',
      stock: 9,
      price: 130,
      status: 'Draft',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['satin', 'new'],
    },
    {
      id: 'prod-011',
      name: 'Pearl Edge Hijab',
      category: 'Hijabs',
      sku: 'SKU-01116',
      stock: 16,
      price: 28,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['pearl', 'edge'],
    },
    {
      id: 'prod-012',
      name: 'Printed Modal Hijab',
      category: 'Hijabs',
      sku: 'SKU-01117',
      stock: 38,
      price: 24,
      status: 'Archived',
      imageUrl:
        'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?auto=format&fit=crop&w=200&q=80',
      tags: ['modal', 'printed'],
    },
    {
      id: 'prod-013',
      name: 'Layered Prayer Set',
      category: 'Prayer Sets',
      sku: 'SKU-01118',
      stock: 27,
      price: 52,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['layered'],
    },
    {
      id: 'prod-014',
      name: 'Chiffon Cape Dress',
      category: 'Dresses',
      sku: 'SKU-01119',
      stock: 12,
      price: 155,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=200&q=80',
      tags: ['cape', 'chiffon'],
    },
    {
      id: 'prod-015',
      name: 'Minimalist Abaya Set',
      category: 'Abayas',
      sku: 'SKU-01120',
      stock: 6,
      price: 92,
      status: 'Archived',
      imageUrl:
        'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=200&q=80',
      tags: ['set', 'minimalist'],
    },
    {
      id: 'prod-016',
      name: 'Crinkle Jersey Hijab',
      category: 'Hijabs',
      sku: 'SKU-01121',
      stock: 5,
      price: 21,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['crinkle', 'jersey'],
    },
    {
      id: 'prod-017',
      name: 'Soft Cotton Abaya',
      category: 'Abayas',
      sku: 'SKU-01122',
      stock: 33,
      price: 79,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=200&q=80',
      tags: ['cotton', 'soft'],
    },
    {
      id: 'prod-018',
      name: 'Structured Modest Dress',
      category: 'Modest Dresses',
      sku: 'SKU-01123',
      stock: 14,
      price: 160,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?auto=format&fit=crop&w=200&q=80',
      tags: ['structured'],
    },
    {
      id: 'prod-019',
      name: 'Premium Prayer Set',
      category: 'Prayer Sets',
      sku: 'SKU-01124',
      stock: 3,
      price: 74,
      status: 'Draft',
      imageUrl:
        'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=200&q=80',
      tags: ['premium'],
    },
    {
      id: 'prod-020',
      name: 'Flowy Satin Dress',
      category: 'Dresses',
      sku: 'SKU-01125',
      stock: 24,
      price: 135,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?auto=format&fit=crop&w=200&q=80',
      tags: ['satin', 'flowy'],
    },
    {
      id: 'prod-021',
      name: 'Everyday Prayer Set',
      category: 'Prayer Sets',
      sku: 'SKU-01126',
      stock: 11,
      price: 48,
      status: 'Active',
      imageUrl:
        'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=200&q=80',
      tags: ['everyday'],
    },
    {
      id: 'prod-022',
      name: 'Seasonal Hijab Set',
      category: 'Hijabs',
      sku: 'SKU-01127',
      stock: 0,
      price: 32,
      status: 'Archived',
      imageUrl:
        'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=200&q=80',
      tags: ['seasonal', 'set'],
    },
  ];

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

  deleteProduct(productId: string): void {
    this.products = this.products.filter((product) => product.id !== productId);
  }

  createProduct(payload: ProductCreatePayload): Observable<Product> {
    const nextId = `prod-${String(this.products.length + 1).padStart(3, '0')}`;
    const mediaUrl = payload.mediaUrls[0];
    const newProduct: Product = {
      id: nextId,
      name: payload.name,
      category: payload.category,
      sku: payload.variants.variantRows[0]?.sku || `SKU-${nextId.toUpperCase()}`,
      stock: payload.variants.variantRows.reduce((sum, row) => sum + (row.quantity ?? 0), 0),
      price: payload.salePrice ?? payload.basePrice,
      status: payload.statusActive ? 'Active' : 'Draft',
      imageUrl: mediaUrl,
      tags: payload.tags,
    };
    this.products = [newProduct, ...this.products];
    return of(newProduct);
  }

  getProductById(productId: string): Observable<Product> {
    const product =
      this.products.find((item) => item.id === productId) ?? this.products[0];

    if (!product) {
      return of({
        id: productId,
        name: 'New Product',
        category: 'Abayas',
        sku: `SKU-${productId.toUpperCase()}`,
        stock: 0,
        price: 0,
        status: 'Draft',
        tags: [],
        description: '',
        basePrice: 0,
        statusActive: false,
        mediaUrls: [],
        variants: [],
      });
    }

    return of(this.buildProductDetail(product));
  }

  updateProduct(productId: string, payload: ProductUpdatePayload): Observable<Product> {
    const existingIndex = this.products.findIndex((item) => item.id === productId);
    const updatedProduct: Product = {
      id: productId,
      name: payload.name,
      category: payload.category,
      sku: payload.variants[0]?.sku ?? `SKU-${productId.toUpperCase()}`,
      stock: payload.variants.reduce((sum, variant) => sum + (variant.inventory ?? 0), 0),
      price: payload.salePrice ?? payload.basePrice,
      status: payload.statusActive ? 'Active' : 'Draft',
      imageUrl: payload.mediaUrls[0],
      tags: payload.tags,
      description: payload.description,
      subCategory: payload.subCategory,
      basePrice: payload.basePrice,
      salePrice: payload.salePrice,
      statusActive: payload.statusActive,
      mediaUrls: payload.mediaUrls,
      variants: payload.variants,
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

  removeProductMedia(productId: string, mediaUrl: string): Observable<boolean> {
    const product = this.products.find((item) => item.id === productId);
    if (!product) {
      return of(false);
    }
    const updatedMedia = (product.mediaUrls ?? []).filter((url) => url !== mediaUrl);
    product.mediaUrls = updatedMedia;
    if (product.imageUrl === mediaUrl) {
      product.imageUrl = updatedMedia[0];
    }
    return of(true);
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
    const variants =
      product.variants?.length && product.variants.length > 0
        ? product.variants
        : [
            {
              label: 'S / Midnight Blue',
              price: basePrice,
              sku: product.sku + '-S',
              inventory: Math.max(0, Math.round(product.stock / 2)),
              imageUrl: mediaUrls[0],
            },
            {
              label: 'M / Midnight Blue',
              price: basePrice,
              sku: product.sku + '-M',
              inventory: Math.max(0, product.stock - Math.round(product.stock / 2)),
              imageUrl: mediaUrls[0],
            },
          ];

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
      variants,
    };
  }
}
