import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Product } from '../../../core/models/product-model';
import { ProductQuery } from '../../../core/models/product-query-model';
import { ProductService } from '../../../core/services/product';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { debounceTime, distinctUntilChanged, Subject, Subscription } from 'rxjs';
import { Modal } from 'bootstrap';
import { StockTransactionService } from '../../../core/services/stock-transaction';
import { CreateStockTransaction } from '../../../core/models/create-stock-transaction.model';
import { ToastService } from '../../../core/services/toast';

@Component({
  selector: 'app-stock-management',
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './stock-management.html',
  styleUrl: './stock-management.css',
})
export class StockManagement implements OnInit, OnDestroy {
  // const productServices=inject(Product);
  constructor(private productServices: ProductService, private stockService: StockTransactionService,
    private toast: ToastService) { }

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;
  private fb = inject(FormBuilder)

  private stockModalInstance?: Modal;


  ngOnInit(): void {
    this.setupSearch();
    this.loadProducts();

    const modalEl = document.getElementById('stockModal');
    if (modalEl) {
      this.stockModalInstance = new Modal(modalEl, { backdrop: true });
      modalEl.addEventListener('hidden.bs.modal', () => {
        this.resetTransactionForm();
      });
    }
  }

  private openModal() {
    this.stockModalInstance?.show();
  }

  private closeModalNow() {
    this.stockModalInstance?.hide();
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe()
  }
  apiBaseUrl = environment.apiBaseUrl;

  //for stockmanagement
  transactionType = signal<"In" | "Out">("In");
  selectedProduct = signal<Product | null>(null);

  products = signal<Product[]>([]);
  loading = signal(false);

  page = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  totalItems = signal(0);

  search = signal("");
  sortBy = signal("name");
  descending = signal(false);

  get query(): ProductQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search()
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

          this.loading.set(false);
          console.log(this.products())
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ?? "Unable to load products"
          );
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
    this.searchSubscription = this.searchSubject
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

  stockForm = this.fb.group({
    quantity: [
      1,
      [Validators.required,
      Validators.min(1)
      ]
    ],
    remarks: [
      ""
    ]
  })

  openStockIn(product: Product) {
    this.selectedProduct.set(product);
    this.transactionType.set("In")

    this.stockForm.reset({
      quantity: 1,
      remarks: ""
    })
    this.openModal();
  }
  openStockOut(product: Product) {
    this.selectedProduct.set(product);
    this.transactionType.set("Out")

    this.stockForm.reset({
      quantity: 1,
      remarks: ""
    })
    this.openModal();
  }

  saveTransaction() {
    // console.log("Reach ed");

    if (this.stockForm.invalid ||
      !this.selectedProduct()
    ) return;

    const request: CreateStockTransaction = {
      productId: this.selectedProduct()!.id,
      type: this.transactionType(),
      quantity: this.stockForm.value.quantity!,
      remarks: this.stockForm.value.remarks ?? ""
    }
    // console.log(request)

    this.stockService.createTransaction(request)
      .subscribe({
        next: (response) => {
          this.products.update(products =>
            products.map(product =>
              product.id === response.data.product.id ? response.data.product : product
            )
          )
          this.toast.success(response.message);
          this.resetTransactionForm();
          this.closeModalNow();
        },
        error: err => {

          this.toast.error(
            err.error?.message ??
            "Transaction failed."
          );

        }
      })
  }

  resetTransactionForm() {
    this.stockForm.reset({
      quantity: 1,
      remarks: ""
    })
    this.selectedProduct.set(null)
  }
}
