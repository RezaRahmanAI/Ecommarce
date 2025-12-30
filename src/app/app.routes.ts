import { Routes } from '@angular/router';

import { HomePageComponent } from './features/home/pages/home-page/home-page.component';
import { MenProductsPageComponent } from './features/men/pages/men-products-page/men-products-page.component';
import { WomenProductsPageComponent } from './features/women/pages/women-products-page/women-products-page.component';
import { ChildrenProductsPageComponent } from './features/children/pages/children-products-page/children-products-page.component';
import { PlaceholderComponent } from './features/placeholder/placeholder.component';
import { AccessoriesPageComponent } from './features/accessories/pages/accessories-page/accessories-page.component';
import { ProductDetailsPageComponent } from './features/product-details/pages/product-details-page/product-details-page.component';
import { CartPageComponent } from './features/cart/pages/cart-page/cart-page.component';
import { CheckoutPageComponent } from './features/checkout/pages/checkout-page/checkout-page.component';
import { OrderConfirmationPageComponent } from './features/order-confirmation/pages/order-confirmation-page/order-confirmation-page.component';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'men', component: MenProductsPageComponent },
  { path: 'women', component: WomenProductsPageComponent },
  { path: 'children', component: ChildrenProductsPageComponent },
  { path: 'accessories', component: AccessoriesPageComponent },
  {
    path: 'product/:id',
    component: ProductDetailsPageComponent,
  },
  { path: 'cart', component: CartPageComponent },
  { path: 'checkout', component: CheckoutPageComponent },
  { path: 'order-confirmation/:orderId', component: OrderConfirmationPageComponent },
  {
    path: 'track/:orderId',
    component: PlaceholderComponent,
    data: { title: 'Track Order', description: 'Order tracking experience coming soon.' },
  },
  { path: 'login', component: PlaceholderComponent, data: { title: 'Login', description: 'Login experience coming soon.' } },
  { path: '**', redirectTo: '' },
];
