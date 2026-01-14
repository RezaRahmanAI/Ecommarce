import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { MOCK_CATEGORIES } from '../data/mock-categories';
import { Category } from '../models/category';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private readonly categories = MOCK_CATEGORIES;

  // private readonly baseUrl = '/api/categories';

  getCategories(): Observable<Category[]> {
    return of(this.categories);
  }
}
