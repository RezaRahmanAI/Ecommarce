export interface Category {
  id: string;
  name: string;
  slug: string;
  parentId?: string | null;
  imageUrl?: string;
  isVisible: boolean;
  productCount: number;
  sortOrder: number;
}

export interface CategoryNode {
  category: Category;
  children: CategoryNode[];
}

export interface ReorderPayload {
  parentId: string | null;
  orderedIds: string[];
}
