import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { combineLatest, map } from 'rxjs';

import { AuthStateService } from '../../core/services/auth-state.service';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  private readonly authState = inject(AuthStateService);
  private readonly cartService = inject(CartService);

  readonly vm$ = combineLatest([this.authState.user$, this.cartService.summary$]).pipe(
    map(([user, summary]) => ({
      user,
      cartCount: summary.itemsCount,
    })),
  );

  logout(): void {
    this.authState.logout();
  }
}
