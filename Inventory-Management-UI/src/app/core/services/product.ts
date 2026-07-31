import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpBackend, HttpClient, HttpParams } from '@angular/common/http';
import { ProductQuery } from '../models/product-query-model';
import { ApiResponse } from '../models/app-response.model';
import { PagedResult } from '../models/paged-result-model';
import { Product } from '../models/product-model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getProducts(query: ProductQuery) {

    let params = new HttpParams()
      .set("page", query.page.toString())
      .set("pageSize", query.pageSize.toString());

    if (query.search) {
      params = params.set(
        'search',
        query.search
      );
    }

    if (query.minPrice != null) {
      params = params.set(
        'minPrice',
        query.minPrice.toString()
      );
    }

    if (query.maxPrice != null) {
      params = params.set(
        'maxPrice',
        query.maxPrice.toString()
      );
    }

    if (query.lowStockOnly != null) {
      params = params.set(
        'lowStockOnly',
        query.lowStockOnly.toString()
      );
    }

    if (query.sortBy) {
      params = params.set(
        'sortBy',
        query.sortBy
      );
    }

    if (query.descending != null) {
      params = params.set(
        'descending',
        query.descending.toString()
      );
    }

    return this.http.get<ApiResponse<PagedResult<Product>>>(
      `${this.apiUrl}/products`,
      {params}
    );
  }
}
