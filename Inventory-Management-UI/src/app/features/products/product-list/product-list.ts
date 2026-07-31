import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Product } from '../../../core/models/product-model';
import { ProductQuery } from '../../../core/models/product-query-model';
import { ProductService } from '../../../core/services/product';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { debounceTime, distinctUntilChanged, Subject, Subscription } from 'rxjs';
import { validate } from '@angular/forms/signals';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList implements OnInit {
  // const productServices=inject(Product);
  constructor(private productServices: ProductService) { }
  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

  ngOnInit(): void {
    this.setupSearch();
    this.loadProducts();
  }
  ngOnDestroy() :void{
    this.searchSubscription?.unsubscribe()
  }

  apiBaseUrl = environment.apiBaseUrl;

  products = signal<Product[]>([]);
  loading = signal(false);
  errorMessage = signal("");
  successMessage = signal("");

  page = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  totalItems = signal(0);

  search = signal("");
  sortBy = signal("");
  descending = signal(false);

  minPrice=signal<number | null>(null);
  maxPrice=signal<number | null>(null);
  lowStockOnly=signal(false);


  get query(): ProductQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search(),
      sortBy: this.sortBy(),
      descending: this.descending(),
      minPrice:this.minPrice()??undefined,
      maxPrice:this.maxPrice  ()??undefined,
      lowStockOnly:this.lowStockOnly()
    };
  };

  loadProducts() {
    this.loading.set(true);

    this.productServices.getProducts(this.query)
      .subscribe({
        next: (response) => {

          this.products.set(response.data.items)
          this.page.set(response.data.page)
          this.pageSize.set(response.data.pageSize)
          this.totalItems.set(response.data.totalItems)
          this.totalPages.set(response.data.totalPages)

          this.successMessage.set(response.message)
          this.errorMessage.set("")
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(
            err.error?.message ?? "Unable to load products"
          )
          this.loading.set(false);
        }
      })
  }

  nextPage() {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1);
      this.loadProducts();
    }
  }
  previousPage() {
    if (this.page() > 1) {
      this.page.update(p => p - 1);
      this.loadProducts();
    }
  }

  goToPage(p: number) {
    if (p == this.page() || p <= 0 || p > this.totalPages()) return;

    this.page.set(p);
    this.loadProducts()
  }

  get pagesList(): number[] {
    return Array.from(
      { length: this.totalPages() },
      (_, i) => i + 1
    );
  }

  changePageSize(event: Event) {
    const size = Number((event.target as HTMLSelectElement).value)

    this.pageSize.set(size);
    this.page.set(1);
    this.loadProducts();
  }

  private setupSearch() {
    this.searchSubscription=this.searchSubject
      .pipe(
        debounceTime(400),
        distinctUntilChanged()
      )
      .subscribe(value => {
        this.search.set(value)
        this.page.set(1)
        this.loadProducts()
      })
  }

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    // this.searchSubject.next(value);
    this.searchSubject.next(value);
    // this.page.set(1);
    // this.loadProducts()
  }

  sort(column:string){
    if(this.sortBy()==column){
      this.descending.update(value=>!value)
    }
    else{
      // this.descending.update(value=>!validate)
      this.sortBy.set(column)
      this.descending.set(false)
    }
    this.page.set(1);
    this.loadProducts();
  }

  changeMinPrice(event:Event){
    const value= Number((event.target as HTMLInputElement).value);
    this.minPrice.set(value || null)
    this.page.set(1)
    this.loadProducts()
  }
  changeMaxPrice(event:Event){
    const value= Number((event.target as HTMLInputElement).value);
    this.maxPrice.set(value || null)
    this.page.set(1)
    this.loadProducts()
  }
  toggleLowStock(event:Event){
    const checked=(event.target as HTMLInputElement).checked
    this.lowStockOnly.set(checked)
    this.page.set(1)
    this.loadProducts()
  }
  clearFilters(){
    this.maxPrice.set(null);
    this.minPrice.set(null);
    this.lowStockOnly.set(false);
    this.search.set("")
    this.sortBy.set("name")
    this.descending.set(false)
    this.page.set(1)
    this.loadProducts()
  }
}
