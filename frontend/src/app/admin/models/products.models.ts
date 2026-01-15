import {
  ProductImage,
  ProductMeta,
  ProductRatings,
  RelatedProduct,
  VariantColor,
  VariantSize,
} from '../../core/models/product';

export type ProductStatus = 'Active' | 'Draft' | 'Archived' | 'Out of Stock';

export interface ProductVariantEdit {
  label: string;
  price: number;
  sku: string;
  inventory: number;
  imageUrl?: string;
}

export interface Product {
  id: number;
  name: string;
  description: string;
  category: string;
  subCategory: string;
  tags: string[];
  badges: string[];
  price: number;
  salePrice?: number;
  purchaseRate?: number;
  gender: 'men' | 'women' | 'kids' | 'accessories';
  ratings: ProductRatings;
  images: {
    mainImage: ProductImage;
    thumbnails: ProductImage[];
  };
  variants: {
    colors: VariantColor[];
    sizes: VariantSize[];
  };
  meta: ProductMeta;
  relatedProducts: RelatedProduct[];
  featured?: boolean;
  newArrival?: boolean;
  sku: string;
  stock: number;
  status: ProductStatus;
  imageUrl?: string;
  statusActive?: boolean;
  mediaUrls?: string[];
  basePrice?: number;
  inventoryVariants?: ProductVariantEdit[];
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
  subCategory: string;
  gender: 'men' | 'women' | 'kids' | 'accessories';
  tags: string[];
  badges: string[];
  price: number;
  salePrice?: number;
  purchaseRate: number;
  featured: boolean;
  newArrival: boolean;
  ratings: ProductRatings;
  media: {
    mainImage: ProductImage;
    thumbnails: ProductImage[];
  };
  variants: {
    colors: VariantColor[];
    sizes: VariantSize[];
  };
  meta: ProductMeta;
}

export interface ProductUpdatePayload {
  name: string;
  description: string;
  statusActive: boolean;
  category: string;
  subCategory?: string;
  gender: 'men' | 'women' | 'kids' | 'accessories';
  tags: string[];
  badges: string[];
  featured: boolean;
  newArrival: boolean;
  basePrice: number;
  salePrice?: number;
  purchaseRate: number;
  mediaUrls: string[];
  inventoryVariants: ProductVariantEdit[];
}

export type ProductsStatusTab = 'All Items' | 'Active' | 'Drafts' | 'Archived';

export interface ProductsQueryParams {
  searchTerm: string;
  category: string;
  statusTab: ProductsStatusTab;
  page: number;
  pageSize: number;
}
