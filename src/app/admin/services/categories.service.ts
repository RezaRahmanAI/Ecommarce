import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { Category, CategoryNode, ReorderPayload } from '../models/categories.models';

@Injectable({
  providedIn: 'root',
})
export class CategoriesService {
  private categories: Category[] = [
    {
      id: 'cat-1',
      name: 'Abayas',
      slug: 'abayas',
      parentId: null,
      description: 'Elegant modest abayas for everyday wear.',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuC9pYfBZcPDiI5ZJ7g9bIgbGImV-Z1wsYdU5hcfBfdEHJUIA0yeF5Q5qQI6mfKh7VdmufIaDi7X23_5EYjDRCi0UCrWC6gbCQ7gNkeI-Epa7SZrHIcx5yyx0wSlEAgaO7zrAXPlRqThlZlECnyTcS74iDBNslyVfA0C63QZsyzh2Gyw1fTMDReYDksXD73-ImXaOfdBYvAJwiVKAssIZxr5QZoMtfn3CdZGQZFpc3Zgb4zmlHn7e7a7a3PecaGoA6ntPBh_8x0wKKA',
      isVisible: true,
      productCount: 154,
      sortOrder: 1,
    },
    {
      id: 'cat-1-1',
      name: 'Open Front Abayas',
      slug: 'abayas/open-front-abayas',
      parentId: 'cat-1',
      description: 'Lightweight open front styles for layering.',
      imageUrl: '',
      isVisible: true,
      productCount: 42,
      sortOrder: 1,
    },
    {
      id: 'cat-1-2',
      name: 'Butterfly Abayas',
      slug: 'abayas/butterfly-abayas',
      parentId: 'cat-1',
      description: 'Wide sleeve butterfly silhouettes.',
      imageUrl: '',
      isVisible: true,
      productCount: 28,
      sortOrder: 2,
    },
    {
      id: 'cat-2',
      name: 'Hijabs',
      slug: 'hijabs',
      parentId: null,
      description: 'Daily essentials for every occasion.',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuCyh4AvQGNIapihCyaGbGouOjjAgGzopp36rET2CWil9wRmjTTBwhVcPgepNyOPBMi_FjTENRkYr0oi4MRzgMtkQN0TET_r16n9n-FMdHg-BWMffB5CsduKF_f-1_y46Hr5nwmdlbZLLCMZU5tEdUV-Qgc7St7Cpp7f6PmXuWs7EkNvvznp6_M40XENtAP2AHuRw8QU3xBLLn0cQkfm7Z1lkhZNDewkW0Y4aB1joQ8hvqoAZfSOSDY9jja-PRqudFCfybtMw5X46hY',
      isVisible: true,
      productCount: 120,
      sortOrder: 2,
    },
    {
      id: 'cat-2-1',
      name: 'Silk Hijabs',
      slug: 'hijabs/silk-hijabs',
      parentId: 'cat-2',
      description: 'Luxurious silk blends in signature shades.',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuDsixXrbLTEQulEO2JHUTqx9QjIIBwROpUY33CR-iaWciLR270YgLMYNlZOb3-dfXPtFupK8pZ2ra_Wg_rXgQX2z4gZduwlmHjPi4UgUYfV_iVH0JJcvhkN8-U5iU1bwEM-gpMj87g87EoisJAsZp-9hpsqYB5G83LHQcC82I_kwViyGE3EgB0tflDNfPQpA2rQCA4Oej9YDRwkPKubcuuYlq1N6xFKCaHcR3nl7CC6bF3bKQfBWrWTsxA4NfHzyr8DnWLoN5lbpEM',
      isVisible: true,
      productCount: 45,
      sortOrder: 1,
    },
    {
      id: 'cat-2-2',
      name: 'Chiffon Hijabs',
      slug: 'hijabs/chiffon-hijabs',
      parentId: 'cat-2',
      description: 'Lightweight chiffon styles for everyday looks.',
      imageUrl: '',
      isVisible: true,
      productCount: 32,
      sortOrder: 2,
    },
    {
      id: 'cat-3',
      name: 'Prayer Sets',
      slug: 'prayer-sets',
      parentId: null,
      description: 'Comfortable sets for prayer time.',
      imageUrl:
        'https://lh3.googleusercontent.com/aida-public/AB6AXuBkKNrLH2kccVXQw-LGx9aFn8RTb6MpnSOqiR3KFGem15Vi2MhsXR0vl22JwuYxPh3_nA3P1so4NkrKOT68KL7vcTFzsIB95gtxjLup28ZXWVz1D7WqcAaoOWbDbNE9PwxHcac4XoVAvZkWOjGZ_eXlVHIpYfaC4Cq_iodTJazXx13JueHNZWg5yj-WRvIjg16MEFSJl_dOtFN18b1t2S-WRS5bI_lq9-8sz7J0OiQyr88KNAWC1O3AtxzRDhFA00I3tMJEfCbsX5U',
      isVisible: true,
      productCount: 8,
      sortOrder: 3,
    },
    {
      id: 'cat-3-1',
      name: 'Travel Prayer Sets',
      slug: 'prayer-sets/travel-sets',
      parentId: 'cat-3',
      description: 'Compact sets for travel and on-the-go.',
      imageUrl: '',
      isVisible: true,
      productCount: 5,
      sortOrder: 1,
    },
  ];

  getAll(): Observable<Category[]> {
    return of(this.cloneCategories());
  }

  getById(id: string): Observable<Category> {
    const category = this.categories.find((item) => item.id === id);
    return of(category ? { ...category } : (null as unknown as Category));
  }

  create(payload: Partial<Category>): Observable<Category> {
    const parentId = payload.parentId ?? null;
    const siblings = this.categories.filter((item) => (item.parentId ?? null) === parentId);
    const nextSortOrder =
      siblings.length > 0 ? Math.max(...siblings.map((item) => item.sortOrder)) + 1 : 1;
    const category: Category = {
      id: `cat-${Date.now()}`,
      name: payload.name ?? 'New Category',
      slug: payload.slug ?? '',
      parentId,
      description: payload.description ?? '',
      imageUrl: payload.imageUrl ?? '',
      isVisible: payload.isVisible ?? true,
      productCount: payload.productCount ?? 0,
      sortOrder: payload.sortOrder ?? nextSortOrder,
    };
    this.categories.push(category);
    return of({ ...category });
  }

  update(id: string, payload: Partial<Category>): Observable<Category> {
    const index = this.categories.findIndex((item) => item.id === id);
    if (index === -1) {
      return of(null as unknown as Category);
    }
    this.categories[index] = {
      ...this.categories[index],
      ...payload,
      parentId: payload.parentId ?? null,
    };
    return of({ ...this.categories[index] });
  }

  delete(id: string): Observable<boolean> {
    const index = this.categories.findIndex((item) => item.id === id);
    if (index === -1) {
      return of(false);
    }
    this.categories.splice(index, 1);
    return of(true);
  }

  uploadImage(file: File): Observable<string> {
    return new Observable((observer) => {
      const reader = new FileReader();
      reader.onload = () => {
        observer.next(reader.result as string);
        observer.complete();
      };
      reader.onerror = () => {
        observer.error('Unable to read file');
      };
      reader.readAsDataURL(file);
    });
  }

  reorder(payload: ReorderPayload): Observable<boolean> {
    const orderedIds = payload.orderedIds;
    this.categories = this.categories.map((item) => {
      const parentMatches = (item.parentId ?? null) === payload.parentId;
      if (!parentMatches) {
        return item;
      }
      const orderIndex = orderedIds.indexOf(item.id);
      if (orderIndex === -1) {
        return item;
      }
      return {
        ...item,
        sortOrder: orderIndex + 1,
      };
    });
    return of(true);
  }

  getTree(): Observable<CategoryNode[]> {
    return of(this.buildTree(this.cloneCategories()));
  }

  private cloneCategories(): Category[] {
    return this.categories.map((item) => ({ ...item }));
  }

  private buildTree(categories: Category[]): CategoryNode[] {
    const grouped = new Map<string | null, Category[]>();
    categories.forEach((category) => {
      const key = category.parentId ?? null;
      if (!grouped.has(key)) {
        grouped.set(key, []);
      }
      grouped.get(key)?.push(category);
    });

    const buildNodes = (parentId: string | null): CategoryNode[] => {
      const items = grouped.get(parentId) ?? [];
      const sorted = [...items].sort((a, b) => a.sortOrder - b.sortOrder);
      return sorted.map((category) => ({
        category,
        children: buildNodes(category.id),
      }));
    };

    return buildNodes(null);
  }
}
