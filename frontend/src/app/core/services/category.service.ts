import { Injectable, inject } from "@angular/core";
import { Observable, map } from "rxjs";

import { ApiHttpClient } from "../http/http-client";
import { Category } from "../models/category";

type AdminCategory = {
  id: string;
  name: string;
  slug: string;
  parentId?: string | null;
  description?: string;
  imageUrl?: string;
  isVisible: boolean;
  productCount: number;
  sortOrder: number;
};

@Injectable({
  providedIn: "root",
})
export class CategoryService {
  private readonly api = inject(ApiHttpClient);

  getCategories(): Observable<Category[]> {
    return this.api.get<AdminCategory[]>("/categories").pipe(
      map((categories) =>
        categories
          .filter((category) => category.isVisible && !category.parentId)
          .sort((first, second) => first.sortOrder - second.sortOrder)
          .map((category) => ({
            id: category.id,
            name: category.name,
            slug: category.slug,
            description: category.description,
            imageUrl: category.imageUrl ?? "",
            href: category.slug
              ? `/products?category=${encodeURIComponent(category.slug)}`
              : "/products",
            productCount: category.productCount,
          })),
      ),
    );
  }
}
