import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product';
import { PriceDisplayComponent } from '../../../../shared/components/price-display/price-display.component';

@Component({
  selector: 'app-men-product-grid',
  standalone: true,
  imports: [CommonModule, RouterLink, PriceDisplayComponent],
  templateUrl: './product-grid.component.html',
  styleUrl: './product-grid.component.css',
})
export class MenProductGridComponent implements OnInit {
  products: Product[] = [];

  constructor(private readonly productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getByGender('men').subscribe((products) => {
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
