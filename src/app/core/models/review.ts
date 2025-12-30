export interface Review {
  id: number;
  productId: number;
  reviewerInitials: string;
  reviewerName: string;
  rating: number;
  title: string;
  message: string;
  timeAgo: string;
}
