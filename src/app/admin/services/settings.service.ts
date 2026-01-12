import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { AdminSettings } from '../models/settings.models';

const MOCK_SETTINGS: AdminSettings = {
  storeName: 'Arza',
  supportEmail: 'support@arza.com',
  description: 'A modern modest clothing brand dedicated to quality and style.',
  stripeEnabled: true,
  paypalEnabled: false,
  stripePublishableKey: 'pk_live_51M...xYz2',
  shippingZones: [
    {
      id: 1,
      name: 'Domestic',
      region: 'United States',
      rates: ['Free Shipping (>$100)', 'Standard: $5.00'],
    },
    {
      id: 2,
      name: 'International',
      region: 'Rest of World',
      rates: ['Flat Rate: $25.00'],
    },
  ],
};

@Injectable({ providedIn: 'root' })
export class SettingsService {
  getSettings(): Observable<AdminSettings> {
    return of(MOCK_SETTINGS);
  }

  saveSettings(payload: AdminSettings): Observable<{ success: true }> {
    return of({ success: true });
  }
}
