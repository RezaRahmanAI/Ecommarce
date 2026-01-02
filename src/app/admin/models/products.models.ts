export type ProductStatus = 'Active' | 'Draft' | 'Archived' | 'Out of Stock';

export interface Product {
  id: string;
  name: string;
  category: 'Abayas' | 'Hijabs' | 'Prayer Sets' | 'Modest Dresses' | 'Dresses' | string;
  sku: string;
  stock: number;
  price: number;
  status: ProductStatus;
  imageUrl?: string;
  tags?: string[];
}

export type ProductsStatusTab = 'All Items' | 'Active' | 'Drafts' | 'Archived';

export interface ProductsQueryParams {
  searchTerm: string;
  category: string;
  statusTab: ProductsStatusTab;
  page: number;
  pageSize: number;
}
