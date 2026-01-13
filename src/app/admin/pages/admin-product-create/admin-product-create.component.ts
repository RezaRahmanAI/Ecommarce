import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { switchMap } from 'rxjs/operators';

import { ProductCreatePayload } from '../../models/products.models';
import { ProductImage } from '../../../core/models/product';
import { ProductsService } from '../../services/products.service';
import { PriceDisplayComponent } from '../../../shared/components/price-display/price-display.component';

interface MediaFormValue {
  id: string;
  url: string;
  label: string;
  alt: string;
  type: 'image' | 'video';
  isMain: boolean;
  source: 'file' | 'url';
}

@Component({
  selector: 'app-admin-product-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, PriceDisplayComponent],
  templateUrl: './admin-product-create.component.html',
})
export class AdminProductCreateComponent implements OnDestroy {
  private formBuilder = inject(FormBuilder);
  private productsService = inject(ProductsService);
  private router = inject(Router);

  private readonly defaultRatings: ProductCreatePayload['ratings'] = {
    avgRating: 0,
    reviewCount: 0,
    ratingBreakdown: [
      { rating: 5, percentage: 0 },
      { rating: 4, percentage: 0 },
      { rating: 3, percentage: 0 },
      { rating: 2, percentage: 0 },
      { rating: 1, percentage: 0 },
    ],
  };

  mediaError = '';
  private mediaFileMap = new Map<string, File>();

  form = this.formBuilder.group(
    {
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required]],
      statusActive: [true],
      category: ['', [Validators.required]],
      subCategory: ['', [Validators.required]],
      gender: ['women', [Validators.required]],
      badges: [''],
      tags: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      salePrice: [null as number | null, [Validators.min(0)]],
      featured: [false],
      newArrival: [false],
      mediaFiles: [[] as File[]],
      mediaItems: this.formBuilder.array([]),
      variants: this.formBuilder.group({
        colors: this.formBuilder.array([this.createColorGroup(true)]),
        sizes: this.formBuilder.array([this.createSizeGroup(true)]),
      }),
      meta: this.formBuilder.group({
        fabricAndCare: [''],
        shippingAndReturns: [''],
      }),
    },
    { validators: [this.salePriceValidator] }
  );

  ngOnDestroy(): void {
    this.mediaItemsArray.controls.forEach((control) => {
      const value = control.value as MediaFormValue;
      if (value.source === 'file') {
        URL.revokeObjectURL(value.url);
      }
    });
    this.mediaFileMap.clear();
  }

  get mediaItemsArray(): FormArray {
    return this.form.get('mediaItems') as FormArray;
  }

  get colorsArray(): FormArray {
    return this.form.get('variants.colors') as FormArray;
  }

  get sizesArray(): FormArray {
    return this.form.get('variants.sizes') as FormArray;
  }

  addColor(): void {
    this.colorsArray.push(this.createColorGroup(false));
  }

  removeColor(index: number): void {
    if (this.colorsArray.length <= 1) {
      return;
    }
    const wasSelected = Boolean(this.colorsArray.at(index)?.get('selected')?.value);
    this.colorsArray.removeAt(index);
    if (wasSelected) {
      this.ensureSingleSelected(this.colorsArray, 'selected');
    }
  }

  setSelectedColor(index: number): void {
    this.ensureSingleSelected(this.colorsArray, 'selected', index);
  }

  addSize(): void {
    this.sizesArray.push(this.createSizeGroup(false));
  }

  removeSize(index: number): void {
    if (this.sizesArray.length <= 1) {
      return;
    }
    const wasSelected = Boolean(this.sizesArray.at(index)?.get('selected')?.value);
    this.sizesArray.removeAt(index);
    if (wasSelected) {
      this.ensureSingleSelected(this.sizesArray, 'selected');
    }
  }

  setSelectedSize(index: number): void {
    this.ensureSingleSelected(this.sizesArray, 'selected', index);
  }

  handleFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      return;
    }
    this.addFiles(Array.from(input.files));
    input.value = '';
  }

  handleDrop(event: DragEvent): void {
    event.preventDefault();
    if (!event.dataTransfer?.files?.length) {
      return;
    }
    this.addFiles(Array.from(event.dataTransfer.files));
  }

  handleDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  addFromUrl(): void {
    const url = window.prompt('Enter media URL');
    if (!url) {
      return;
    }
    this.addMediaItem({
      url,
      source: 'url',
    });
  }

  removeMediaItem(index: number): void {
    const control = this.mediaItemsArray.at(index);
    if (!control) {
      return;
    }
    const value = control.value as MediaFormValue;
    if (value.source === 'file') {
      URL.revokeObjectURL(value.url);
      this.mediaFileMap.delete(value.id);
    }
    this.mediaItemsArray.removeAt(index);
    this.ensureMainMedia();
    this.syncMediaFiles();
  }

  setMainMedia(index: number): void {
    this.ensureSingleSelected(this.mediaItemsArray, 'isMain', index);
  }

  discard(): void {
    const confirmed = window.confirm('Discard changes?');
    if (!confirmed) {
      return;
    }
    this.resetForm();
    void this.router.navigate(['/admin/products']);
  }

  saveProduct(): void {
    this.mediaError = '';

    if (this.mediaItemsArray.length === 0) {
      this.mediaError = 'Add at least one image or video for the product.';
    }

    if (this.form.invalid || this.mediaItemsArray.length === 0) {
      this.form.markAllAsTouched();
      return;
    }

    const files = this.getSelectedFiles();

    this.productsService
      .uploadProductMedia(files)
      .pipe(switchMap((mediaUrls) => this.productsService.createProduct(this.buildPayload(mediaUrls))))
      .subscribe(() => {
        window.alert('Product created successfully.');
        void this.router.navigate(['/admin/products']);
      });
  }

  trackByIndex(index: number): number {
    return index;
  }

  private createColorGroup(selected: boolean): AbstractControl {
    return this.formBuilder.group({
      name: [''],
      hex: ['#111827'],
      selected: [selected],
    });
  }

  private createSizeGroup(selected: boolean): AbstractControl {
    return this.formBuilder.group({
      label: [''],
      stock: [0, [Validators.min(0)]],
      selected: [selected],
    });
  }

  private createMediaItemGroup(item: MediaFormValue): AbstractControl {
    return this.formBuilder.group({
      id: [item.id],
      url: [item.url],
      label: [item.label],
      alt: [item.alt],
      type: [item.type],
      isMain: [item.isMain],
      source: [item.source],
    });
  }

  private salePriceValidator(control: AbstractControl): ValidationErrors | null {
    const basePrice = Number(control.get('price')?.value ?? 0);
    const salePriceControl = control.get('salePrice');
    const salePrice = salePriceControl?.value;
    if (salePrice === null || salePrice === undefined) {
      return null;
    }
    const saleValue = Number(salePrice);
    if (Number.isNaN(saleValue)) {
      return null;
    }
    return saleValue > basePrice ? { salePriceExceedsBase: true } : null;
  }

  private addFiles(files: File[]): void {
    files.forEach((file) => {
      const id = this.generateId('media');
      const url = URL.createObjectURL(file);
      this.mediaFileMap.set(id, file);
      this.addMediaItem({
        id,
        url,
        label: this.titleize(file.name.replace(/\.[^.]+$/, '')) || 'Gallery image',
        alt: this.form.get('name')?.value || 'Product image',
        type: 'image',
        isMain: this.mediaItemsArray.length === 0,
        source: 'file',
      });
    });
  }

  private addMediaItem(partial: Partial<MediaFormValue> & Pick<MediaFormValue, 'url' | 'source'>): void {
    const item: MediaFormValue = {
      id: partial.id ?? this.generateId('media'),
      url: partial.url,
      label: partial.label ?? 'Gallery image',
      alt: partial.alt ?? this.form.get('name')?.value ?? 'Product image',
      type: partial.type ?? 'image',
      isMain: partial.isMain ?? this.mediaItemsArray.length === 0,
      source: partial.source,
    };
    this.mediaItemsArray.push(this.createMediaItemGroup(item));
    this.mediaError = '';
    this.ensureMainMedia();
    this.syncMediaFiles();
  }

  private ensureMainMedia(): void {
    const hasMain = this.mediaItemsArray.controls.some((control) => control.get('isMain')?.value);
    if (!hasMain && this.mediaItemsArray.length > 0) {
      this.mediaItemsArray.at(0)?.get('isMain')?.setValue(true);
    }
  }

  private ensureSingleSelected(array: FormArray, controlName: string, selectedIndex?: number): void {
    array.controls.forEach((control, index) => {
      control.get(controlName)?.setValue(selectedIndex === index, { emitEvent: false });
    });
    if (selectedIndex === undefined && array.length > 0) {
      array.at(0)?.get(controlName)?.setValue(true, { emitEvent: false });
    }
  }

  private syncMediaFiles(): void {
    const files = this.getSelectedFiles();
    this.form.patchValue({ mediaFiles: files });
  }

  private getSelectedFiles(): File[] {
    return Array.from(this.mediaFileMap.values());
  }

  private buildPayload(uploadedUrls: string[]): ProductCreatePayload {
    const raw = this.form.getRawValue();
    const tags = (raw.tags ?? '')
      .split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);
    const badges = (raw.badges ?? '')
      .split(',')
      .map((badge) => badge.trim())
      .filter((badge) => badge.length > 0);

    const mediaItems = this.buildMediaItems(uploadedUrls);
    const mainMedia = mediaItems.find((item) => item.isMain) ?? mediaItems[0];
    const mainImage: ProductImage = mainMedia
      ? this.mapToProductImage(mainMedia)
      : {
          type: 'image',
          label: 'Main',
          url: 'https://via.placeholder.com/600x800?text=Product+Image',
          alt: raw.name ?? 'Product image',
        };
    const thumbnails = mediaItems
      .filter((item) => item.id !== mainMedia?.id)
      .map((item) => this.mapToProductImage(item));

    if (thumbnails.length === 0) {
      thumbnails.push(mainImage);
    }

    const colors = this.colorsArray.controls.map((control, index) => ({
      name: control.get('name')?.value ?? `Color ${index + 1}`,
      hex: control.get('hex')?.value ?? '#111827',
      selected: Boolean(control.get('selected')?.value),
    }));

    const sizes = this.sizesArray.controls.map((control, index) => ({
      label: control.get('label')?.value ?? `Size ${index + 1}`,
      stock: Number(control.get('stock')?.value ?? 0),
      selected: Boolean(control.get('selected')?.value),
    }));

    if (!colors.some((color) => color.selected) && colors.length > 0) {
      colors[0].selected = true;
    }

    if (!sizes.some((size) => size.selected) && sizes.length > 0) {
      sizes[0].selected = true;
    }

    const gender = (raw.gender ?? 'women') as ProductCreatePayload['gender'];

    return {
      name: raw.name ?? '',
      description: raw.description ?? '',
      statusActive: Boolean(raw.statusActive),
      category: raw.category ?? '',
      subCategory: raw.subCategory ?? '',
      gender,
      tags,
      badges,
      price: Number(raw.price ?? 0),
      salePrice: raw.salePrice === null ? undefined : Number(raw.salePrice),
      featured: Boolean(raw.featured),
      newArrival: Boolean(raw.newArrival),
      ratings: this.defaultRatings,
      media: {
        mainImage,
        thumbnails,
      },
      variants: {
        colors,
        sizes,
      },
      meta: {
        fabricAndCare: raw.meta?.fabricAndCare ?? '',
        shippingAndReturns: raw.meta?.shippingAndReturns ?? '',
      },
    };
  }

  private buildMediaItems(uploadedUrls: string[]): MediaFormValue[] {
    let fileIndex = 0;
    return this.mediaItemsArray.controls.map((control) => {
      const value = control.getRawValue() as MediaFormValue;
      if (value.source === 'file') {
        const url = uploadedUrls[fileIndex] ?? value.url;
        fileIndex += 1;
        return { ...value, url };
      }
      return value;
    });
  }

  private mapToProductImage(item: MediaFormValue): ProductImage {
    return {
      type: item.type,
      label: item.label || 'Gallery',
      url: item.url,
      alt: item.alt || 'Product image',
    };
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      description: '',
      statusActive: true,
      category: '',
      subCategory: '',
      gender: 'women',
      badges: '',
      tags: '',
      price: 0,
      salePrice: null,
      featured: false,
      newArrival: false,
      mediaFiles: [],
      mediaItems: [],
      variants: {
        colors: [{ name: '', hex: '#111827', selected: true }],
        sizes: [{ label: '', stock: 0, selected: true }],
      },
      meta: {
        fabricAndCare: '',
        shippingAndReturns: '',
      },
    });
    this.mediaError = '';

    this.mediaItemsArray.controls.forEach((control) => {
      if (control.get('source')?.value === 'file') {
        URL.revokeObjectURL(control.get('url')?.value);
      }
    });
    this.mediaItemsArray.clear();
    this.mediaFileMap.clear();

    while (this.colorsArray.length > 1) {
      this.colorsArray.removeAt(0, { emitEvent: false });
    }
    this.colorsArray.at(0)?.patchValue({ name: '', hex: '#111827', selected: true });

    while (this.sizesArray.length > 1) {
      this.sizesArray.removeAt(0, { emitEvent: false });
    }
    this.sizesArray.at(0)?.patchValue({ label: '', stock: 0, selected: true });
  }

  private titleize(value: string): string {
    return value
      .split(/[-_ ]+/)
      .map((segment) => (segment ? segment[0].toUpperCase() + segment.slice(1) : ''))
      .join(' ')
      .trim();
  }

  private generateId(prefix: string): string {
    return `${prefix}-${Math.random().toString(36).slice(2, 10)}`;
  }
}
