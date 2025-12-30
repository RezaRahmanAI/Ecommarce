import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product';

@Component({
  selector: 'app-accessories-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './accessories-page.component.html',
  styleUrl: './accessories-page.component.css',
})
export class AccessoriesPageComponent implements OnInit {
  products: Product[] = [];

  constructor(private readonly productService: ProductService) {}

  ngOnInit(): void {
    this.productService
      .getByCategory('Accessories')
      .subscribe((products: Product[]) => {
      this.products = products;
    });
  }

  getBadge(product: Product): string | undefined {
    return product.badges[0];
  }
}
