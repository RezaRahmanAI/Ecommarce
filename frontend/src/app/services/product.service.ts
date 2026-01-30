import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

export interface Product {
  id: number;
  name: string;
  description?: string;
  category?: string;
  subCategory?: string;
  tags: string[];
  badges: string[];
  price: number;
  salePrice?: number;
  purchaseRate?: number;
  basePrice: number;
  gender?: string;
  featured: boolean;
  newArrival: boolean;
  sku: string;
  stock: number;
  status: string;
  imageUrl?: string;
  statusActive: boolean;
  mediaUrls: string[];
}

@Injectable({
  providedIn: "root",
})
export class ProductService {
  private readonly API_URL = `${environment.apiUrl}/api/products`;
  private readonly ADMIN_API_URL = `${environment.apiUrl}/api/admin/products`;

  constructor(private http: HttpClient) {}

  // Public APIs (No auth required)
  getProducts(
    searchTerm?: string,
    category?: string,
    page = 1,
    pageSize = 20,
  ): Observable<Product[]> {
    let params = new HttpParams()
      .set("page", page.toString())
      .set("pageSize", pageSize.toString());

    if (searchTerm) {
      params = params.set("searchTerm", searchTerm);
    }
    if (category) {
      params = params.set("category", category);
    }

    return this.http.get<Product[]>(this.API_URL, { params });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.API_URL}/${id}`);
  }

  getFeaturedProducts(limit = 10): Observable<Product[]> {
    const params = new HttpParams().set("limit", limit.toString());
    return this.http.get<Product[]>(`${this.API_URL}/featured`, { params });
  }

  getNewArrivals(limit = 10): Observable<Product[]> {
    const params = new HttpParams().set("limit", limit.toString());
    return this.http.get<Product[]>(`${this.API_URL}/new-arrivals`, { params });
  }

  // Admin APIs (Auth required)
  getAdminProducts(
    searchTerm?: string,
    category?: string,
    statusTab?: string,
    page = 1,
    pageSize = 20,
  ): Observable<Product[]> {
    let params = new HttpParams()
      .set("page", page.toString())
      .set("pageSize", pageSize.toString());

    if (searchTerm) params = params.set("searchTerm", searchTerm);
    if (category) params = params.set("category", category);
    if (statusTab) params = params.set("statusTab", statusTab);

    return this.http.get<Product[]>(this.ADMIN_API_URL, { params });
  }

  createProduct(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(this.ADMIN_API_URL, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.ADMIN_API_URL}/${id}`);
  }
}
