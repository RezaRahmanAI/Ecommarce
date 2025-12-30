import { Routes } from '@angular/router';

import { HomePageComponent } from './features/home/pages/home-page/home-page.component';
import { MenProductsPageComponent } from './features/men/pages/men-products-page/men-products-page.component';
import { WomenProductsPageComponent } from './features/women/pages/women-products-page/women-products-page.component';
import { ChildrenProductsPageComponent } from './features/children/pages/children-products-page/children-products-page.component';
import { PlaceholderComponent } from './features/placeholder/placeholder.component';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'men', component: MenProductsPageComponent },
  { path: 'women', component: WomenProductsPageComponent },
  { path: 'children', component: ChildrenProductsPageComponent },
  {
    path: 'product/:id',
    component: PlaceholderComponent,
    data: { title: 'Product Details', description: 'Product detail view coming soon.' },
  },
  { path: 'cart', component: PlaceholderComponent, data: { title: 'Cart', description: 'Cart experience coming soon.' } },
  { path: 'login', component: PlaceholderComponent, data: { title: 'Login', description: 'Login experience coming soon.' } },
  { path: '**', redirectTo: '' },
];
