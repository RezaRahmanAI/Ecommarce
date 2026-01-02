import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-product-edit',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-product-edit.component.html',
})
export class AdminProductEditComponent {
  private route = inject(ActivatedRoute);

  productId = this.route.snapshot.paramMap.get('id');
}
