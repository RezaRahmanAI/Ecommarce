import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AdminSettings, ShippingZone } from '../../models/settings.models';
import { SettingsService } from '../../services/settings.service';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './admin-settings.component.html',
  styleUrl: './admin-settings.component.css',
})
export class AdminSettingsComponent implements OnInit {
  private formBuilder = inject(NonNullableFormBuilder);
  private settingsService = inject(SettingsService);

  @ViewChild('fileUpload') fileUpload?: ElementRef<HTMLInputElement>;

  tabs = ['General', 'User Roles', 'Payments', 'Shipping', 'Notifications'];
  activeTab = 'General';

  settingsForm = this.formBuilder.group({
    storeName: ['', Validators.required],
    supportEmail: ['', [Validators.required, Validators.email]],
    description: [''],
    payments: this.formBuilder.group({
      stripeEnabled: [false],
      paypalEnabled: [false],
    }),
  });

  zoneForm = this.formBuilder.group({
    id: [0],
    name: ['', Validators.required],
    region: ['', Validators.required],
    rates: ['', Validators.required],
  });

  shippingZones: ShippingZone[] = [];
  stripePublishableKey = '';
  logoPreviewUrl: string | null = null;
  logoError = '';
  saveMessage = '';
  showZoneForm = false;
  editingZoneId: number | null = null;
  copiedKey = false;

  private lastSettings: AdminSettings | null = null;

  ngOnInit(): void {
    this.settingsService.getSettings().subscribe((settings) => {
      this.lastSettings = settings;
      this.shippingZones = [...settings.shippingZones];
      this.stripePublishableKey = settings.stripePublishableKey;
      this.settingsForm.patchValue({
        storeName: settings.storeName,
        supportEmail: settings.supportEmail,
        description: settings.description,
        payments: {
          stripeEnabled: settings.stripeEnabled,
          paypalEnabled: settings.paypalEnabled,
        },
      });
    });
  }

  get stripeEnabled(): boolean {
    return this.settingsForm.controls.payments.controls.stripeEnabled.value;
  }

  get paypalEnabled(): boolean {
    return this.settingsForm.controls.payments.controls.paypalEnabled.value;
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  saveChanges(): void {
    this.saveMessage = '';

    if (this.settingsForm.invalid) {
      this.settingsForm.markAllAsTouched();
      return;
    }

    const formValue = this.settingsForm.getRawValue();
    const payload: AdminSettings = {
      storeName: formValue.storeName,
      supportEmail: formValue.supportEmail,
      description: formValue.description,
      stripeEnabled: formValue.payments.stripeEnabled,
      paypalEnabled: formValue.payments.paypalEnabled,
      stripePublishableKey: this.stripePublishableKey,
      shippingZones: [...this.shippingZones],
    };

    this.settingsService.saveSettings(payload).subscribe((settings) => {
      this.saveMessage = 'Settings saved successfully.';
      this.lastSettings = settings;
      this.shippingZones = [...settings.shippingZones];
    });
  }

  resetForm(): void {
    if (!this.lastSettings) {
      return;
    }

    this.settingsForm.reset({
      storeName: this.lastSettings.storeName,
      supportEmail: this.lastSettings.supportEmail,
      description: this.lastSettings.description,
      payments: {
        stripeEnabled: this.lastSettings.stripeEnabled,
        paypalEnabled: this.lastSettings.paypalEnabled,
      },
    });

    this.shippingZones = [...this.lastSettings.shippingZones];
    this.stripePublishableKey = this.lastSettings.stripePublishableKey;
    this.saveMessage = '';
  }

  triggerLogoUpload(): void {
    this.fileUpload?.nativeElement.click();
  }

  onLogoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    const isValidType = ['image/png', 'image/jpeg', 'image/gif'].includes(file.type);
    const isValidSize = file.size <= 2 * 1024 * 1024;

    if (!isValidType || !isValidSize) {
      this.logoError = 'Please upload a PNG, JPG, or GIF file up to 2MB.';
      this.logoPreviewUrl = null;
      input.value = '';
      return;
    }

    this.logoError = '';
    const reader = new FileReader();
    reader.onload = () => {
      this.logoPreviewUrl = typeof reader.result === 'string' ? reader.result : null;
    };
    reader.readAsDataURL(file);
  }

  copyStripeKey(): void {
    if (!this.stripePublishableKey) {
      return;
    }

    navigator.clipboard.writeText(this.stripePublishableKey).then(() => {
      this.copiedKey = true;
      setTimeout(() => {
        this.copiedKey = false;
      }, 2000);
    });
  }

  startAddZone(): void {
    this.showZoneForm = true;
    this.editingZoneId = null;
    this.zoneForm.reset({ id: 0, name: '', region: '', rates: '' });
  }

  editZone(zone: ShippingZone): void {
    this.showZoneForm = true;
    this.editingZoneId = zone.id;
    this.zoneForm.reset({
      id: zone.id,
      name: zone.name,
      region: zone.region,
      rates: zone.rates.join(', '),
    });
  }

  saveZone(): void {
    if (this.zoneForm.invalid) {
      this.zoneForm.markAllAsTouched();
      return;
    }

    const formValue = this.zoneForm.getRawValue();
    const updatedZone: ShippingZone = {
      id: formValue.id ?? 0,
      name: formValue.name,
      region: formValue.region,
      rates: formValue.rates
        .split(',')
        .map((rate) => rate.trim())
        .filter(Boolean),
    };

    if (this.editingZoneId) {
      this.settingsService
        .updateShippingZone(this.editingZoneId, updatedZone)
        .subscribe((zone) => {
          this.shippingZones = this.shippingZones.map((item) =>
            item.id === zone.id ? zone : item,
          );
          this.zoneForm.reset({ id: 0, name: '', region: '', rates: '' });
          this.showZoneForm = false;
          this.editingZoneId = null;
        });
      return;
    }

    this.settingsService.createShippingZone(updatedZone).subscribe((zone) => {
      this.shippingZones = [...this.shippingZones, zone];
      this.zoneForm.reset({ id: 0, name: '', region: '', rates: '' });
      this.showZoneForm = false;
      this.editingZoneId = null;
    });
  }

  cancelZoneEdit(): void {
    this.zoneForm.reset({ id: 0, name: '', region: '', rates: '' });
    this.showZoneForm = false;
    this.editingZoneId = null;
  }

  deleteZone(zone: ShippingZone): void {
    if (!confirm(`Delete the ${zone.name} zone?`)) {
      return;
    }

    this.settingsService.deleteShippingZone(zone.id).subscribe((success) => {
      if (!success) {
        return;
      }
      this.shippingZones = this.shippingZones.filter((item) => item.id !== zone.id);
    });
  }

  rateClass(rate: string): string {
    if (rate.toLowerCase().includes('free shipping')) {
      return 'px-2 py-1 bg-accent/30 text-primary text-xs rounded-md font-medium';
    }

    return 'px-2 py-1 bg-gray-100 text-text-secondary text-xs rounded-md font-medium';
  }

}
