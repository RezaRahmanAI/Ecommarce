export interface CheckoutState {
  fullName: string;
  phone: string;
  address: string;
  deliveryDetails: string;
}

export interface ShippingMethod {
  id: string;
  label: string;
  description: string;
  price: number;
  estimatedDelivery: string;
}
