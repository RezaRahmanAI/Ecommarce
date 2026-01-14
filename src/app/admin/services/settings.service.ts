import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { AdminSettings, ShippingZone } from '../models/settings.models';
import { ApiHttpClient } from '../../core/http/http-client';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly api = inject(ApiHttpClient);

  getSettings(): Observable<AdminSettings> {
    return this.api.get<AdminSettings>('/admin/settings');
  }

  saveSettings(payload: AdminSettings): Observable<AdminSettings> {
    return this.api.put<AdminSettings>('/admin/settings', payload);
  }

  createShippingZone(payload: ShippingZone): Observable<ShippingZone> {
    return this.api.post<ShippingZone>('/admin/settings/shipping-zones', payload);
  }

  updateShippingZone(zoneId: number, payload: ShippingZone): Observable<ShippingZone> {
    return this.api.put<ShippingZone>(`/admin/settings/shipping-zones/${zoneId}`, payload);
  }

  deleteShippingZone(zoneId: number): Observable<boolean> {
    return this.api.delete<boolean>(`/admin/settings/shipping-zones/${zoneId}`);
  }
}
