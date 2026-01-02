export interface ShippingZone {
  id: number;
  name: string;
  region: string;
  rates: string[];
}

export interface AdminSettings {
  storeName: string;
  supportEmail: string;
  description: string;
  stripeEnabled: boolean;
  paypalEnabled: boolean;
  stripePublishableKey: string;
  shippingZones: ShippingZone[];
}
