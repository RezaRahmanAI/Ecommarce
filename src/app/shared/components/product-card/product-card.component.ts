import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Product } from '../../../core/models/product';
import { BadgeComponent } from '../badge/badge.component';
import { IconButtonComponent } from '../icon-button/icon-button.component';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, BadgeComponent, IconButtonComponent],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css',
})
export class ProductCardComponent {
  @Input({ required: true }) product!: Product;
}
