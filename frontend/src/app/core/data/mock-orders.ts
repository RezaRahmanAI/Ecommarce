import { Order, OrderStatus } from '../models/order';

export const MOCK_ORDERS: Order[] = [
  {
    id: 'MSLM-890234',
    status: OrderStatus.Processing,
    items: [
      {
        productId: 1,
        name: 'Modest Silk Abaya',
        price: 120,
        quantity: 1,
        color: 'Navy Blue',
        size: 'Medium',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuBdCokpUwyySv_CwFuQed1QC7hxqnQtjP8DG-04T1HpbkVvXoprIdTNo8qGa99JAHBdp4Nb_IAj48tLc60-77pxh1CpwM_ECcXrxzIeouzGVCoGpDykBggPD2fajBbqEw30ckrXgyQBys2UAHvYmII6SmOH3fHMeD70gGLgvwUxX6oHrTFxycgY03X6O-y0VjkRgFtGZKDNWlXzvUn2_jJ8W2iaNfSP7ahIyBp4r8lQcZh0y4yCpAOckDekotOe07ST3U5d-zi9Xxs',
        imageAlt: 'Woman wearing elegant modest navy blue dress',
        sku: 'MS-AB-001',
      },
      {
        productId: 2,
        name: 'Premium Chiffon Hijab',
        price: 25,
        quantity: 2,
        color: 'Beige',
        size: 'One Size',
        imageUrl:
          'https://lh3.googleusercontent.com/aida-public/AB6AXuCbnsCt3QIKY5UNSA6-NhEGRLenR5dhk7TGZuUu-Y2Go0wcAVmbeGB8m6W3l-_IByKd6OQGbgwaxi7PWbWY37IKSVDOFNhNAUtiSDptavRmRNBOeP1TEf-PwbdabY7mFU7ol0BWAnRJVpeE8VgrnYjCED-6WCnJGr7uOxTncQ51x7eTgqN0QpWo1hJVVNMlcGUtXDXf_jtyh6uHkTEm5Pf4nUrS12M-MfFnEtEneZfn8kgjkio92zJHijlLbFOtdqQEwn0qZZ_rTk8',
        imageAlt: 'Beige premium chiffon hijab folded neatly',
        sku: 'MS-HJ-042',
      },
    ],
    customer: {
      name: 'Sarah Ahmed',
      phone: '+1 (555) 201-8890',
      address: '123 Olive Grove Avenue, Apt 4B, New York, NY 10012',
      deliveryDetails: 'Leave at the front desk, call on arrival.',
    },
    totals: {
      subtotal: 170,
      shipping: 0,
      tax: 14.45,
      total: 184.45,
    },
    timeline: {
      confirmedDate: 'Oct 24',
      processingLabel: 'In Progress',
      shippedEta: 'Est. Oct 26',
      deliveredEta: 'Est. Oct 28',
    },
  },
];
