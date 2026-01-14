import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-order-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-order-details.component.html',
})
export class AdminOrderDetailsComponent {
  private route = inject(ActivatedRoute);

  orderId = this.route.snapshot.paramMap.get('id');
}
