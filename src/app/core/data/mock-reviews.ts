import { Review } from '../models/review';

export const MOCK_REVIEWS: Review[] = [
  {
    id: 1,
    productId: 1,
    reviewerInitials: 'SA',
    reviewerName: 'Sarah A.',
    rating: 5,
    title: 'Absolutely stunning quality!',
    message: `The silk is so soft and opaque, exactly what I was looking for. The sizing is perfect (I got a Medium and I'm 5'6"). Will definitely order in other colors.`,
    timeAgo: '2 days ago',
  },
  {
    id: 2,
    productId: 1,
    reviewerInitials: 'FK',
    reviewerName: 'Fatima K.',
    rating: 4,
    title: 'Beautiful but slightly long',
    message:
      'The color is exactly as pictured, a very deep beautiful blue. It was a bit long for me but easily hemmed. Very elegant flow.',
    timeAgo: '1 week ago',
  },
];
