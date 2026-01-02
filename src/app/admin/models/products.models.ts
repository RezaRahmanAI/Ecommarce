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

export interface ProductVariantOption {
  optionName: 'Size' | 'Color' | 'Material' | string;
  values: string;
}

export interface ProductVariantRow {
  label: string;
  price: number;
  sku: string;
  quantity: number;
}

export interface ProductCreatePayload {
  name: string;
  description: string;
  statusActive: boolean;
  category: string;
  subcategory?: string;
  collections?: string;
  tags: string[];
  basePrice: number;
  salePrice?: number;
  mediaUrls: string[];
  variants: {
    options: ProductVariantOption[];
    variantRows: ProductVariantRow[];
  };
}

export type ProductsStatusTab = 'All Items' | 'Active' | 'Drafts' | 'Archived';

export interface ProductsQueryParams {
  searchTerm: string;
  category: string;
  statusTab: ProductsStatusTab;
  page: number;
  pageSize: number;
}
