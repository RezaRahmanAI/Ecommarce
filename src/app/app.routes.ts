import { Routes } from '@angular/router';

import { AccessoriesPageComponent } from './features/accessories/pages/accessories-page/accessories-page.component';
import { AccountPageComponent } from './features/account/pages/account-page/account-page.component';
import { CartPageComponent } from './features/cart/pages/cart-page/cart-page.component';
import { ChildrenProductsPageComponent } from './features/children/pages/children-products-page/children-products-page.component';
import { CheckoutPageComponent } from './features/checkout/pages/checkout-page/checkout-page.component';
import { HomePageComponent } from './features/home/pages/home-page/home-page.component';
import { LoginPageComponent } from './features/login/pages/login-page/login.page';
import { MenProductsPageComponent } from './features/men/pages/men-products-page/men-products-page.component';
import { OrderConfirmationPageComponent } from './features/order-confirmation/pages/order-confirmation-page/order-confirmation-page.component';
import { OrdersPageComponent } from './features/orders/pages/orders-page/orders-page.component';
import { PlaceholderComponent } from './features/placeholder/placeholder.component';
import { BlogDetailsComponent } from './features/blog/pages/blog-details/blog-details.component';
import { BlogListComponent } from './features/blog/pages/blog-list/blog-list.component';
import { ProductDetailsPageComponent } from './features/product-details/pages/product-details-page/product-details-page.component';
import { RegisterPageComponent } from './features/register/pages/register-page/register.page';
import { WomenProductsPageComponent } from './features/women/pages/women-products-page/women-products-page.component';
import { AdminLayoutComponent } from './admin/layout/admin-layout/admin-layout.component';
import { DashboardOverviewComponent } from './admin/pages/dashboard-overview/dashboard-overview.component';
import { AdminPlaceholderComponent } from './admin/pages/admin-placeholder/admin-placeholder.component';
import { AdminLogoutComponent } from './admin/pages/admin-logout/admin-logout.component';
import { AdminOrderDetailsComponent } from './admin/pages/admin-order-details/admin-order-details.component';
import { AdminOrdersComponent } from './admin/pages/admin-orders/admin-orders.component';
import { AdminProductEditComponent } from './admin/pages/admin-product-edit/admin-product-edit.component';
import { AdminProductCreateComponent } from './admin/pages/admin-product-create/admin-product-create.component';
import { AdminProductsComponent } from './admin/pages/admin-products/admin-products.component';
import { AdminCategoryManagementComponent } from './admin/pages/admin-category-management/admin-category-management.component';
import { AdminSettingsComponent } from './admin/pages/admin-settings/admin-settings.component';
import { AboutComponent } from './pages/about/about.component';
import { ContactComponent } from './pages/contact/contact.component';
import { authGuard } from './core/guards/auth.guard';
import { AdminGuard } from './admin/guards/admin.guard';

export const appRoutes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'men', component: MenProductsPageComponent },
  { path: 'women', component: WomenProductsPageComponent },
  { path: 'children', component: ChildrenProductsPageComponent },
  { path: 'accessories', component: AccessoriesPageComponent },
  { path: 'products', redirectTo: 'women', pathMatch: 'full' },
  { path: 'about', component: AboutComponent, title: 'About Us' },
  { path: 'contact', component: ContactComponent, title: 'Contact' },
  {
    path: 'product/:id',
    component: ProductDetailsPageComponent,
  },
  { path: 'cart', component: CartPageComponent },
  { path: 'checkout', component: CheckoutPageComponent, canActivate: [authGuard] },
  { path: 'order-confirmation/:orderId', component: OrderConfirmationPageComponent },
  {
    path: 'track/:orderId',
    component: PlaceholderComponent,
    data: { title: 'Track Order', description: 'Order tracking experience coming soon.' },
  },
  { path: 'login', component: LoginPageComponent },
  {
    path: 'register',
    component: RegisterPageComponent,
  },
  {
    path: 'forgot-password',
    component: PlaceholderComponent,
    data: { title: 'Forgot Password', description: 'Password recovery experience coming soon.' },
  },
  {
    path: 'blog',
    component: BlogListComponent,
  },
  {
    path: 'blog/:slug',
    component: BlogDetailsComponent,
  },
  {
    path: 'account',
    component: AccountPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'orders',
    component: OrdersPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [AdminGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        component: DashboardOverviewComponent,
        data: { title: 'Dashboard Overview' },
      },
      {
        path: 'products',
        component: AdminProductsComponent,
        data: { title: 'Products' },
      },
      {
        path: 'products/categories',
        component: AdminCategoryManagementComponent,
        data: { title: 'Category Management' },
      },
      {
        path: 'products/create',
        component: AdminProductCreateComponent,
        data: { title: 'Add Product' },
      },
      {
        path: 'products/:id/edit',
        component: AdminProductEditComponent,
        data: { title: 'Edit Product' },
      },
      {
        path: 'orders',
        component: AdminOrdersComponent,
        data: { title: 'Order Management' },
      },
      {
        path: 'orders/:id',
        component: AdminOrderDetailsComponent,
        data: { title: 'Order Details' },
      },
      {
        path: 'customers',
        component: AdminPlaceholderComponent,
        data: { title: 'Customers', description: 'Customer management coming soon.' },
      },
      {
        path: 'analytics',
        component: AdminPlaceholderComponent,
        data: { title: 'Analytics', description: 'Analytics reports coming soon.' },
      },
      {
        path: 'settings',
        component: AdminSettingsComponent,
        data: { title: 'Settings' },
      },
      {
        path: 'logout',
        component: AdminLogoutComponent,
        data: { title: 'Logging out' },
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
