import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject, combineLatest } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';

import {
  ProductCreatePayload,
  ProductVariantOption,
  ProductVariantRow,
} from '../../models/products.models';
import { ProductsService } from '../../services/products.service';

interface MediaPreview {
  id: string;
  url: string;
  file?: File;
  isExternal?: boolean;
}

@Component({
  selector: 'app-admin-product-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './admin-product-create.component.html',
})
export class AdminProductCreateComponent implements OnInit, OnDestroy {
  private formBuilder = inject(FormBuilder);
  private productsService = inject(ProductsService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  mediaPreviews: MediaPreview[] = [];
  variantLimitReached = false;

  form = this.formBuilder.group(
    {
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required]],
      statusActive: [true],
      category: ['', [Validators.required]],
      subcategory: [''],
      collections: [''],
      tags: [''],
      basePrice: [0, [Validators.required, Validators.min(0)]],
      salePrice: [null as number | null, [Validators.min(0)]],
      mediaFiles: [[] as File[]],
      variants: this.formBuilder.group({
        options: this.formBuilder.array([this.createOptionGroup()]),
        variantRows: this.formBuilder.array([]),
      }),
    },
    { validators: [this.salePriceValidator] }
  );

  ngOnInit(): void {
    combineLatest([
      this.optionsArray.valueChanges.pipe(startWith(this.optionsArray.value)),
      this.form.get('basePrice')!.valueChanges.pipe(
        startWith(this.form.get('basePrice')!.value)
      ),
    ])
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.refreshVariantRows();
      });

    this.variantRowsArray.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.form.updateValueAndValidity({ emitEvent: false });
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.mediaPreviews.forEach((preview) => {
      if (preview.file) {
        URL.revokeObjectURL(preview.url);
      }
    });
  }

  get optionsArray(): FormArray {
    return this.form.get('variants.options') as FormArray;
  }

  get variantRowsArray(): FormArray {
    return this.form.get('variants.variantRows') as FormArray;
  }

  get duplicateSkus(): Set<string> {
    const counts = new Map<string, number>();
    this.variantRowsArray.controls.forEach((control) => {
      const value = (control.get('sku')?.value ?? '').toString().trim();
      if (!value) {
        return;
      }
      counts.set(value, (counts.get(value) ?? 0) + 1);
    });
    return new Set([...counts.entries()].filter(([, count]) => count > 1).map(([sku]) => sku));
  }

  addOption(): void {
    this.optionsArray.push(this.createOptionGroup());
  }

  removeOption(index: number): void {
    if (this.optionsArray.length <= 1) {
      return;
    }
    this.optionsArray.removeAt(index);
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
    const url = window.prompt('Enter image URL');
    if (!url) {
      return;
    }
    this.mediaPreviews.push({ id: this.generateId('media'), url, isExternal: true });
  }

  removeMedia(preview: MediaPreview): void {
    if (preview.file) {
      URL.revokeObjectURL(preview.url);
    }
    this.mediaPreviews = this.mediaPreviews.filter((item) => item.id !== preview.id);
    this.syncMediaFiles();
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
    if (this.form.invalid || this.hasDuplicateSkus()) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.buildPayload();
    const files = this.getSelectedFiles();

    this.productsService
      .uploadProductMedia(files)
      .pipe(
        switchMap((mediaUrls) =>
          this.productsService.createProduct({
            ...payload,
            mediaUrls: [...mediaUrls, ...payload.mediaUrls],
          })
        )
      )
      .subscribe(() => {
        window.alert('Product created successfully.');
        void this.router.navigate(['/admin/products']);
      });
  }

  trackByIndex(index: number): number {
    return index;
  }

  private createOptionGroup(): AbstractControl {
    return this.formBuilder.group({
      optionName: ['Size'],
      values: [''],
    });
  }

  private refreshVariantRows(): void {
    const options = this.optionsArray.controls.map((control) => ({
      optionName: control.get('optionName')?.value ?? '',
      values: control.get('values')?.value ?? '',
    }));

    const combinations = this.generateVariantCombinations(options);
    const basePrice = Number(this.form.get('basePrice')?.value ?? 0);

    this.variantLimitReached = combinations.length > 50;
    const limited = combinations.slice(0, 50);

    const existing = new Map<string, ProductVariantRow>();
    this.variantRowsArray.controls.forEach((control) => {
      const row = control.value as ProductVariantRow;
      if (row.label) {
        existing.set(row.label, row);
      }
    });

    while (this.variantRowsArray.length) {
      this.variantRowsArray.removeAt(0, { emitEvent: false });
    }

    limited.forEach((label) => {
      const previous = existing.get(label);
      const price =
        previous?.price ??
        (Number.isFinite(basePrice) && basePrice >= 0 ? basePrice : 0);
      const rowGroup = this.formBuilder.group({
        label: [label],
        price: [price, [Validators.min(0)]],
        sku: [previous?.sku ?? ''],
        quantity: [previous?.quantity ?? 0, [Validators.min(0)]],
      });
      this.variantRowsArray.push(rowGroup, { emitEvent: false });
    });

    this.form.updateValueAndValidity({ emitEvent: false });
  }

  private generateVariantCombinations(options: ProductVariantOption[]): string[] {
    const sanitized = options
      .map((option) => ({
        name: option.optionName?.trim(),
        values: this.parseValues(option.values),
      }))
      .filter((option) => option.values.length > 0);

    if (sanitized.length === 0) {
      return [];
    }

    return sanitized.reduce<string[]>((acc, option) => {
      const next: string[] = [];
      option.values.forEach((value) => {
        acc.forEach((existing) => {
          next.push(existing ? `${existing} / ${value}` : value);
        });
      });
      return next.length ? next : acc;
    }, ['']);
  }

  private parseValues(values: string): string[] {
    return values
      .split(',')
      .map((value) => value.trim())
      .filter((value) => value.length > 0);
  }

  private salePriceValidator(control: AbstractControl): ValidationErrors | null {
    const basePrice = Number(control.get('basePrice')?.value ?? 0);
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
    const newPreviews = files.map((file) => ({
      id: this.generateId('file'),
      url: URL.createObjectURL(file),
      file,
    }));
    this.mediaPreviews = [...this.mediaPreviews, ...newPreviews];
    this.syncMediaFiles();
  }

  private syncMediaFiles(): void {
    const files = this.getSelectedFiles();
    this.form.patchValue({ mediaFiles: files });
  }

  private getSelectedFiles(): File[] {
    return this.mediaPreviews.filter((preview) => preview.file).map((preview) => preview.file!);
  }

  private buildPayload(): ProductCreatePayload {
    const raw = this.form.getRawValue();
    const tags = (raw.tags ?? '')
      .split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);
    const variantRows = this.variantRowsArray.controls.map((control) => {
      const row = control.value as ProductVariantRow;
      return {
        label: row.label,
        price: Number(row.price ?? 0),
        sku: row.sku ?? '',
        quantity: Number(row.quantity ?? 0),
      };
    });

    const options = this.optionsArray.controls.map((control) => {
      const option = control.value as ProductVariantOption;
      return {
        optionName: option.optionName ?? '',
        values: option.values ?? '',
      };
    });

    const externalUrls = this.mediaPreviews
      .filter((preview) => preview.isExternal)
      .map((preview) => preview.url);

    return {
      name: raw.name ?? '',
      description: raw.description ?? '',
      statusActive: Boolean(raw.statusActive),
      category: raw.category ?? '',
      subcategory: raw.subcategory ?? '',
      collections: raw.collections ?? '',
      tags,
      basePrice: Number(raw.basePrice ?? 0),
      salePrice: raw.salePrice === null ? undefined : Number(raw.salePrice),
      mediaUrls: externalUrls,
      variants: {
        options,
        variantRows,
      },
    };
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      description: '',
      statusActive: true,
      category: '',
      subcategory: '',
      collections: '',
      tags: '',
      basePrice: 0,
      salePrice: null,
      mediaFiles: [],
      variants: {
        options: [{ optionName: 'Size', values: '' }],
        variantRows: [],
      },
    });
    this.mediaPreviews.forEach((preview) => {
      if (preview.file) {
        URL.revokeObjectURL(preview.url);
      }
    });
    this.mediaPreviews = [];
    while (this.optionsArray.length > 1) {
      this.optionsArray.removeAt(0, { emitEvent: false });
    }
    this.optionsArray.at(0)?.patchValue({ optionName: 'Size', values: '' });
    while (this.variantRowsArray.length) {
      this.variantRowsArray.removeAt(0, { emitEvent: false });
    }
  }

  private hasDuplicateSkus(): boolean {
    return this.duplicateSkus.size > 0;
  }

  private generateId(prefix: string): string {
    return `${prefix}-${Math.random().toString(36).slice(2, 10)}`;
  }
}
