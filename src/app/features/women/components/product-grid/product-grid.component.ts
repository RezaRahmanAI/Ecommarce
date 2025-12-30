import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product';

@Component({
  selector: 'app-women-product-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-grid.component.html',
  styleUrl: './product-grid.component.css',
})
export class WomenProductGridComponent implements OnInit {
  products: Product[] = [];

  constructor(private readonly productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getByGender('women').subscribe((products) => {
      this.products = products;
    });
  }

  getBadge(product: Product): string | undefined {
    return product.badges[0];
  }

  getSelectedColorName(product: Product): string {
    return product.variants.colors.find((color) => color.selected)?.name ?? '';
  }
}
