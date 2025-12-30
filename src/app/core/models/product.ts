export type ProductBadge = 'New' | 'Trending' | 'Sale' | 'Bestseller' | 'Just In';

export interface Product {
  id: number;
  name: string;
  description?: string;
  category: string;
  subcategory?: string;
  price: number;
  salePrice?: number;
  color?: string;
  material?: string;
  rating?: number;
  badge?: ProductBadge;
  imageUrl: string;
  gender?: 'men' | 'women' | 'kids' | 'accessories';
  featured?: boolean;
  newArrival?: boolean;
}
