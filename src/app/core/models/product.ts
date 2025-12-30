export interface ProductImage {
  type: 'image' | 'video';
  label: string;
  url: string;
  alt: string;
}

export interface VariantColor {
  name: string;
  hex: string;
  selected: boolean;
}

export interface VariantSize {
  label: string;
  stock: number;
  selected: boolean;
}

export interface RatingBreakdown {
  rating: 1 | 2 | 3 | 4 | 5;
  percentage: number;
}

export interface ProductRatings {
  avgRating: number;
  reviewCount: number;
  ratingBreakdown: RatingBreakdown[];
}

export interface ProductMeta {
  fabricAndCare: string;
  shippingAndReturns: string;
}

export interface RelatedProduct {
  id: number;
  name: string;
  price: number;
  image: ProductImage;
  quickViewLabel: string;
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
  gender?: 'men' | 'women' | 'kids' | 'accessories';
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
}
