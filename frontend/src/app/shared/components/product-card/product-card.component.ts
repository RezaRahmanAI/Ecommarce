import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Product } from '../../../core/models/product';
import { BadgeComponent } from '../badge/badge.component';
import { IconButtonComponent } from '../icon-button/icon-button.component';
import { PriceDisplayComponent } from '../price-display/price-display.component';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterLink, BadgeComponent, IconButtonComponent, PriceDisplayComponent],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css',
})
export class ProductCardComponent {
  @Input({ required: true }) product!: Product;

  get primaryBadge(): string | undefined {
    return this.product.badges[0];
  }

  get selectedColorName(): string {
    return this.product.variants.colors.find((color) => color.selected)?.name ?? '';
  }
}
