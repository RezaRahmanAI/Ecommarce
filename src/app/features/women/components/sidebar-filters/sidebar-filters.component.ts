import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product';

interface CategoryCount {
  label: string;
  count: number;
  highlight?: boolean;
}

@Component({
  selector: 'app-women-sidebar-filters',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar-filters.component.html',
  styleUrl: './sidebar-filters.component.css',
})
export class WomenSidebarFiltersComponent implements OnInit {
  categoryCounts: CategoryCount[] = [];

  constructor(private readonly productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getByGender('women').subscribe((products) => {
      this.categoryCounts = this.buildCategoryCounts(products);
    });
  }

  private buildCategoryCounts(products: Product[]): CategoryCount[] {
    const categoryOrder = ['Abayas', 'Dresses', 'Hijabs', 'Tops', 'Bottoms'];
    const counts = products.reduce<Record<string, number>>((acc, product) => {
      acc[product.subCategory] = (acc[product.subCategory] ?? 0) + 1;
      return acc;
    }, {});

    return [
      { label: 'All Products', count: products.length, highlight: true },
      ...categoryOrder.map((label) => ({
        label,
        count: counts[label] ?? 0,
      })),
    ];
  }
}
