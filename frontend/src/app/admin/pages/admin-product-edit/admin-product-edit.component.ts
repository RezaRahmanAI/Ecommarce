import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { Product, ProductUpdatePayload, ProductVariantEdit } from '../../models/products.models';
import { ProductsService } from '../../services/products.service';
import { PriceDisplayComponent } from '../../../shared/components/price-display/price-display.component';

@Component({
  selector: 'app-admin-product-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, PriceDisplayComponent],
  templateUrl: './admin-product-edit.component.html',
})
export class AdminProductEditComponent implements OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productsService = inject(ProductsService);
  private formBuilder = inject(FormBuilder);

  productId: number | null = null;
  productName = 'Product';
  categoryName = 'Category';

  mediaNewPreviews: Array<{ file: File; url: string }> = [];
  private originalSnapshot: ProductUpdatePayload | null = null;

  form = this.formBuilder.group(
    {
      name: this.formBuilder.control('', [Validators.required, Validators.minLength(3)]),
      description: this.formBuilder.control('', Validators.required),
      statusActive: this.formBuilder.control(true),
      basePrice: this.formBuilder.control(0, [Validators.required, Validators.min(0)]),
      salePrice: this.formBuilder.control<number | null>(null, [Validators.min(0)]),
      purchaseRate: this.formBuilder.control(0, [Validators.required, Validators.min(0)]),
      category: this.formBuilder.control('', Validators.required),
      subCategory: this.formBuilder.control(''),
      gender: this.formBuilder.control('women', Validators.required),
      badges: this.formBuilder.control(''),
      featured: this.formBuilder.control(false),
      newArrival: this.formBuilder.control(false),
      tags: this.formBuilder.control<string[]>([]),
      tagInput: this.formBuilder.control(''),
      mediaExisting: this.formBuilder.control<string[]>([]),
      mediaNewFiles: this.formBuilder.control<File[]>([]),
      variants: this.formBuilder.array<FormGroup>([]),
    },
    { validators: [this.salePriceValidator.bind(this), this.uniqueSkuValidator.bind(this)] },
  );

  categoryOptions = ['Women', 'Men', 'Kids', 'Accessories'];

  get variants(): FormArray<FormGroup> {
    return this.form.get('variants') as FormArray<FormGroup>;
  }

  get existingMedia(): string[] {
    return this.form.get('mediaExisting')?.value ?? [];
  }

  get statusLabel(): string {
    return this.form.get('statusActive')?.value ? 'Published' : 'Draft';
  }

  get displayName(): string {
    return this.form.get('name')?.value || this.productName;
  }

  get displayCategory(): string {
    return this.form.get('category')?.value || this.categoryName;
  }

  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) {
        return;
      }
      const parsedId = Number(id);
      if (!Number.isFinite(parsedId)) {
        return;
      }
      this.productId = parsedId;
      this.loadProduct(parsedId);
    });
  }

  ngOnDestroy(): void {
    this.mediaNewPreviews.forEach((preview) => URL.revokeObjectURL(preview.url));
  }

  loadProduct(productId: number): void {
    this.productsService.getProductById(productId).subscribe((product) => {
      this.productName = product.name;
      this.categoryName = product.category;
      const payload = this.mapProductToPayload(product);
      this.originalSnapshot = payload;
      this.applyPayloadToForm(payload);
    });
  }

  addTagFromInput(event?: Event): void {
    event?.preventDefault();
    const currentTags = this.form.get('tags')?.value ?? [];
    const inputControl = this.form.get('tagInput');
    const rawValue = String(inputControl?.value ?? '').trim();
    if (!rawValue) {
      return;
    }
    if (!currentTags.includes(rawValue)) {
      this.form.get('tags')?.setValue([...currentTags, rawValue]);
    }
    inputControl?.setValue('');
  }

  removeTag(tag: string): void {
    const currentTags = this.form.get('tags')?.value ?? [];
    this.form.get('tags')?.setValue(currentTags.filter((item) => item !== tag));
  }

  onMediaSelected(event: Event): void {
    const input = event.target as HTMLInputElement | null;
    if (!input?.files?.length) {
      return;
    }
    const files = Array.from(input.files);
    const currentFiles = this.form.get('mediaNewFiles')?.value ?? [];
    const nextFiles = [...currentFiles, ...files];
    this.form.get('mediaNewFiles')?.setValue(nextFiles);
    files.forEach((file) => {
      this.mediaNewPreviews.push({ file, url: URL.createObjectURL(file) });
    });
    input.value = '';
  }

  removeExistingMedia(mediaUrl: string): void {
    if (this.productId === null) {
      return;
    }
    const updatedMedia = this.existingMedia.filter((url) => url !== mediaUrl);
    this.form.get('mediaExisting')?.setValue(updatedMedia);
    this.productsService.removeProductMedia(this.productId, mediaUrl).subscribe();
  }

  removeNewMedia(index: number): void {
    const previews = [...this.mediaNewPreviews];
    const [removed] = previews.splice(index, 1);
    if (removed) {
      URL.revokeObjectURL(removed.url);
    }
    this.mediaNewPreviews = previews;
    const currentFiles = this.form.get('mediaNewFiles')?.value ?? [];
    const updatedFiles = currentFiles.filter((_, fileIndex) => fileIndex !== index);
    this.form.get('mediaNewFiles')?.setValue(updatedFiles);
  }

  downloadAllMedia(): void {
    this.existingMedia.forEach((url) => {
      window.open(url, '_blank');
    });
  }

  addVariant(): void {
    const label = window.prompt('Variant label (e.g., S / Midnight Blue)')?.trim();
    if (!label) {
      return;
    }
    const sku = window.prompt('Variant SKU')?.trim() || `SKU-${Date.now()}`;
    const basePrice = Number(this.form.get('basePrice')?.value ?? 0);
    this.variants.push(this.buildVariantGroup({ label, sku, price: basePrice, inventory: 0 }));
  }

  removeVariant(index: number): void {
    this.variants.removeAt(index);
    this.form.updateValueAndValidity();
  }

  isSkuDuplicate(index: number): boolean {
    const currentSku = this.variants.at(index)?.get('sku')?.value;
    if (!currentSku) {
      return false;
    }
    const skuList = this.variants.controls.map((control) => control.get('sku')?.value);
    return skuList.filter((sku) => sku === currentSku).length > 1;
  }

  toggleStatus(): void {
    const current = this.form.get('statusActive')?.value ?? false;
    this.form.get('statusActive')?.setValue(!current);
  }

  discardChanges(): void {
    if (!window.confirm('Discard changes and return to products?')) {
      return;
    }
    this.resetFormToSnapshot();
    this.router.navigate(['/admin/products']);
  }

  saveChanges(): void {
    if (this.productId === null) {
      return;
    }
    const productId = this.productId;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const newFiles = this.form.get('mediaNewFiles')?.value ?? [];
    this.productsService.uploadProductMedia(newFiles).subscribe((uploadedUrls) => {
      const mediaUrls = [...this.existingMedia, ...uploadedUrls];
      const payload = this.buildUpdatePayload(mediaUrls);
      this.productsService.updateProduct(productId, payload).subscribe(() => {
        window.alert('Product updated successfully.');
        this.router.navigate(['/admin/products']);
      });
    });
  }

  private mapProductToPayload(product: Product): ProductUpdatePayload {
    return {
      name: product.name ?? '',
      description: product.description ?? '',
      statusActive: product.statusActive ?? product.status === 'Active',
      category: product.category ?? '',
      subCategory: product.subCategory ?? '',
      tags: product.tags ?? [],
      badges: product.badges ?? [],
      gender: product.gender ?? 'women',
      featured: product.featured ?? false,
      newArrival: product.newArrival ?? false,
      basePrice: product.basePrice ?? product.price ?? 0,
      salePrice: product.salePrice ?? undefined,
      purchaseRate: product.purchaseRate ?? product.basePrice ?? product.price ?? 0,
      mediaUrls: product.mediaUrls ?? (product.imageUrl ? [product.imageUrl] : []),
      inventoryVariants: product.inventoryVariants ?? [],
    };
  }

  private applyPayloadToForm(payload: ProductUpdatePayload): void {
    this.form.patchValue({
      name: payload.name,
      description: payload.description,
      statusActive: payload.statusActive,
      basePrice: payload.basePrice,
      salePrice: payload.salePrice ?? null,
      purchaseRate: payload.purchaseRate,
      category: payload.category,
      subCategory: payload.subCategory ?? '',
      gender: payload.gender,
      badges: payload.badges.join(', '),
      featured: payload.featured,
      newArrival: payload.newArrival,
      tags: payload.tags,
      mediaExisting: payload.mediaUrls,
      mediaNewFiles: [],
      tagInput: '',
    });

    this.variants.clear();
    payload.inventoryVariants.forEach((variant) => this.variants.push(this.buildVariantGroup(variant)));
    if (payload.inventoryVariants.length === 0) {
      this.variants.push(
        this.buildVariantGroup({
          label: 'Default',
          price: payload.basePrice,
          sku: `SKU-${Date.now()}`,
          inventory: 0,
        }),
      );
    }
    this.mediaNewPreviews.forEach((preview) => URL.revokeObjectURL(preview.url));
    this.mediaNewPreviews = [];
    this.form.updateValueAndValidity();
  }

  private resetFormToSnapshot(): void {
    if (!this.originalSnapshot) {
      return;
    }
    this.applyPayloadToForm(this.originalSnapshot);
  }

  private buildUpdatePayload(mediaUrls: string[]): ProductUpdatePayload {
    const basePrice = Number(this.form.get('basePrice')?.value ?? 0);
    const salePriceControl = this.form.get('salePrice')?.value;
    const salePrice =
      salePriceControl === null || salePriceControl === undefined
        ? undefined
        : Number(salePriceControl);
    const variants = this.variants.controls.map((control) => ({
      label: String(control.get('label')?.value ?? ''),
      price: Number(control.get('price')?.value ?? basePrice),
      sku: String(control.get('sku')?.value ?? ''),
      inventory: Number(control.get('inventory')?.value ?? 0),
      imageUrl: control.get('imageUrl')?.value ?? undefined,
    }));
    const badges = String(this.form.get('badges')?.value ?? '')
      .split(',')
      .map((badge) => badge.trim())
      .filter((badge) => badge.length > 0);
    const gender = (this.form.get('gender')?.value ?? 'women') as ProductUpdatePayload['gender'];

    return {
      name: String(this.form.get('name')?.value ?? ''),
      description: String(this.form.get('description')?.value ?? ''),
      statusActive: Boolean(this.form.get('statusActive')?.value),
      category: String(this.form.get('category')?.value ?? ''),
      subCategory: String(this.form.get('subCategory')?.value ?? ''),
      gender,
      tags: this.form.get('tags')?.value ?? [],
      badges,
      featured: Boolean(this.form.get('featured')?.value),
      newArrival: Boolean(this.form.get('newArrival')?.value),
      basePrice,
      salePrice,
      purchaseRate: Number(this.form.get('purchaseRate')?.value ?? basePrice),
      mediaUrls,
      inventoryVariants: variants,
    };
  }

  private buildVariantGroup(variant: ProductVariantEdit): FormGroup {
    return this.formBuilder.group({
      label: [variant.label ?? ''],
      price: [variant.price ?? 0, [Validators.min(0)]],
      sku: [variant.sku ?? '', [Validators.required]],
      inventory: [variant.inventory ?? 0, [Validators.min(0)]],
      imageUrl: [variant.imageUrl ?? undefined],
    });
  }

  private salePriceValidator(control: AbstractControl): ValidationErrors | null {
    const basePrice = Number(control.get('basePrice')?.value ?? 0);
    const saleValue = control.get('salePrice')?.value;
    if (saleValue === null || saleValue === undefined || saleValue === '') {
      return null;
    }
    const salePrice = Number(saleValue);
    if (Number.isNaN(salePrice) || salePrice < 0) {
      return { salePriceInvalid: true };
    }
    if (salePrice > basePrice) {
      return { salePriceInvalid: true };
    }
    return null;
  }

  private uniqueSkuValidator(control: AbstractControl): ValidationErrors | null {
    const variants = control.get('variants') as FormArray<FormGroup>;
    if (!variants) {
      return null;
    }
    const skuList = variants.controls
      .map((variant) => String(variant.get('sku')?.value ?? '').trim())
      .filter(Boolean);
    const hasDuplicate = skuList.some(
      (sku, index) => skuList.indexOf(sku) !== index,
    );
    return hasDuplicate ? { duplicateSkus: true } : null;
  }
}
