export interface ShippingAddress {
  firstName: string;
  lastName: string;
  address: string;
  apartment?: string;
  city: string;
  region: string;
  postalCode: string;
  country: string;
}

export interface ShippingMethod {
  id: string;
  label: string;
  description: string;
  price: number;
  estimatedDelivery: string;
}

export interface PaymentDetails {
  cardholderName: string;
  cardNumber: string;
  expMonth: string;
  expYear: string;
  cvc: string;
  saveCard: boolean;
}

export interface CheckoutState {
  currentStep: number;
  email: string;
  newsletter: boolean;
  shippingAddress: ShippingAddress;
  shippingMethodId: string;
  promoCode: string;
  payment: PaymentDetails;
}
