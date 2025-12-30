import { ShippingMethod } from '../models/checkout';

export const SHIPPING_METHODS: ShippingMethod[] = [
  {
    id: 'standard',
    label: 'Standard Shipping',
    description: '5-7 business days',
    price: 0,
    estimatedDelivery: '5-7 business days',
  },
  {
    id: 'express',
    label: 'Express Shipping',
    description: '1-2 business days',
    price: 15,
    estimatedDelivery: '1-2 business days',
  },
];
