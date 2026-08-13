import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Product } from '../../../core/models/product-model';
import { ProductQuery } from '../../../core/models/product-query-model';
import { ProductService } from '../../../core/services/product';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { debounceTime, distinctUntilChanged, Subject, Subscription } from 'rxjs';
import { Modal } from 'bootstrap';
import { ToastService } from '../../../core/services/toast';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList implements OnInit, OnDestroy {
  // const productServices=inject(Product);
  constructor(private productServices: ProductService, private toast: ToastService,
    private authService: AuthService) { }
  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;
  private fb = inject(FormBuilder);
  private productModalInstance?: Modal;

  private buildFormData(): FormData {

    const formData = new FormData();

    formData.append(
      'name',
      this.productForm.value.name!
    );

    formData.append(
      'sku',
      this.productForm.value.sku!
    );

    formData.append(
      'description',
      this.productForm.value.description ?? ''
    );

    formData.append(
      'price',
      this.productForm.value.price!.toString()
    );

    formData.append(
      'reorderLevel',
      this.productForm.value.reorderLevel!.toString()
    );

    if (!this.isEditMode()) {

      formData.append(
        'initialQuantity',
        this.productForm.value.initialQuantity!.toString()
      );

    }

    if (this.selectedImage()) {

      formData.append(
        'image',
        this.selectedImage()!
      );

    }

    return formData;

  }


  ngOnInit(): void {
    this.setupSearch();
    this.loadProducts();

    const modalEl = document.getElementById('productModal');

    if (modalEl) {
      this.productModalInstance = new Modal(modalEl, {
        backdrop: true
      });

      modalEl.addEventListener('hidden.bs.modal', () => {
        this.resetForm();
      });
    }
  }
  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe()
  }

  apiBaseUrl = environment.apiBaseUrl;

  products = signal<Product[]>([]);
  loading = signal(false);
  // errorMessage = signal("");
  // successMessage = signal("");

  page = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  totalItems = signal(0);

  search = signal("");
  currentSort = signal('name-asc');
  selectedProduct = signal<Product | null>(null);
  sortBy = signal("name");
  descending = signal(false);

  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  lowStockOnly = signal(false);

  //neeche sirf signals for modal hai

  isEditMode = signal(false)
  editingProductId = signal<number | null>(null)
  selectedImage = signal<File | null>(null)
  imagePreview = signal<string | null>(null)

  //neece delete ke liye signals hai 
  deletingProductId = signal<number | null>(null);
  deletingProductName = signal("");

  get query(): ProductQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search(),
      sortBy: this.sortBy(),
      descending: this.descending(),
      minPrice: this.minPrice() ?? undefined,
      maxPrice: this.maxPrice() ?? undefined,
      lowStockOnly: this.lowStockOnly()
    };
  };
  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

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
          // console.log(this.products())
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ?? "Unable to load products"
          )
          this.loading.set(false);
        }
      })
  }

  openProductModal() {
    this.resetForm();
    this.productModalInstance?.show();
  }

  private closeProductModal() {
    this.productModalInstance?.hide();
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
  changeSort(event: Event) {
    const value =
      (event.target as HTMLSelectElement).value;
    this.currentSort.set(value);
    const [column, direction] =
      value.split('-');
    this.sortBy.set(column);
    this.descending.set(
      direction === 'desc'
    );
    this.page.set(1);
    this.loadProducts();
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

  changeMinPrice(event: Event) {
    const value = Number((event.target as HTMLInputElement).value);
    this.minPrice.set(value || null)
    this.page.set(1)
    this.loadProducts()
  }
  changeMaxPrice(event: Event) {
    const value = Number((event.target as HTMLInputElement).value);
    this.maxPrice.set(value || null)
    this.page.set(1)
    this.loadProducts()
  }
  toggleLowStock(event: Event) {
    const checked = (event.target as HTMLInputElement).checked
    this.lowStockOnly.set(checked)
    this.page.set(1)
    this.loadProducts()
  }
  clearFilters() {
    this.maxPrice.set(null);
    this.minPrice.set(null);
    this.lowStockOnly.set(false);
    this.search.set("");
    this.sortBy.set("name");
    this.descending.set(false);
    this.currentSort.set("name-asc");
    this.page.set(1);
    this.loadProducts();
  }

  //Modal Part

  productForm = this.fb.group({
    name: [
      "",
      Validators.required
    ],
    sku: [
      "",
      Validators.required
    ],
    description: [
      ""
    ],
    price: [
      0, [
        Validators.required,
        Validators.min(0)
      ]
    ],
    initialQuantity: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    reorderLevel: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],
  })
  onImageSelected(event: Event) {

    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0]
    this.selectedImage.set(file)

    const reader = new FileReader();

    reader.onload = () => {
      this.imagePreview.set(
        reader.result as string
      );
    }

    reader.readAsDataURL(file)

  }

  resetForm() {

    this.productForm.reset({
      price: 0,
      initialQuantity: 0,
      reorderLevel: 0
    })

    this.selectedImage.set(null);
    this.imagePreview.set(null);
    this.isEditMode.set(false);
    this.editingProductId.set(null);
  }

  createProduct() {

    if (this.productForm.invalid)
      return;

    const formData = this.buildFormData();

    this.productServices
      .createProduct(formData)
      .subscribe({

        next: response => {
          this.toast.success(response.message);
          this.closeProductModal();
          this.loadProducts();
        },

        error: err => {
          this.toast.error(
            err.error?.message ??
            "Unable to create product"
          );
        }
      });
  }

  editProduct(product: Product) {
    this.isEditMode.set(true);
    this.editingProductId.set(product.id);
    this.selectedImage.set(null);
    this.productForm.patchValue({
      name: product.name,
      sku: product.sku,
      description: product.description,
      price: product.price,
      reorderLevel: product.reorderLevel
    });
    this.imagePreview.set(product.imageUrl ? this.apiBaseUrl + product.imageUrl : null);
  }

  saveProduct() {
    if (this.isEditMode()) {
      this.updateProduct();
    }
    else {
      this.createProduct();
    }
  }
  updateProduct() {
    console.log("updateProduct called");
    if (this.productForm.invalid)
      return;
    const formData = this.buildFormData();
    this.productServices
      .updateProduct(this.editingProductId()!, formData)
      .subscribe({
        next: (response) => {
          this.products.update(products =>
            products.map(product =>
              product.id === response.data.id ? response.data : product
            )
          )
          this.toast.success(response.message);
          this.resetForm();
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            "Unable to update product"
          );
        }
      })
  }
  confirmDelete(product: Product) {
    this.deletingProductId.set(product.id);
    this.deletingProductName.set(product.name);
  }
  deleteProduct() {

    const id = this.deletingProductId();

    if (id == null)
      return;
    this.productServices

      .deleteProduct(id)

      .subscribe({
        next: () => {
          this.products.update(products =>
            products.filter(product =>
              product.id !== id
            )
          );
          this.totalItems.update(
            value => value - 1
          );
          this.toast.success(
            "Product deleted successfully."
          );
          this.deletingProductId.set(null);
          this.deletingProductName.set("");
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            "Unable to delete product."
          );
        }
      });
  }
  showDescription(product: Product) {
    this.selectedProduct.set(product);
  }
  getDescriptionPreview(description: string): string {
    if (!description) {
      return '';
    }
    if (description.length <= 80) {
      return description;
    }
    return description.substring(0, 80) + '...';

  }
}
