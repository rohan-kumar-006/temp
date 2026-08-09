import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { TransactionHistoryQuery } from '../../core/models/transaction-history-query.model';
import { StockTransactionService } from '../../core/services/stock-transaction';
import { ToastService } from '../../core/services/toast';
import { debounceTime, distinctUntilChanged, Subject, Subscription } from 'rxjs';
import { TransactionHistory } from '../../core/models/transaction-history.model';

@Component({
  selector: 'app-transaction-history',
  imports: [CommonModule],
  templateUrl: './transaction-history.html',
  styleUrl: './transaction-history.css',
})
export class TransactionHistoryComponent implements OnInit, OnDestroy {

  constructor(private stockTransactionService: StockTransactionService,
    private toast: ToastService
  ) { }

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

  transactions = signal<TransactionHistory[]>([]);
  loading = signal(false)
  page = signal(1)
  pageSize = signal(10)
  totalItems = signal(0);
  totalPages = signal(0)
  search = signal("")
  type = signal<number | null>(null)
  date = signal("")
  selectedRemark = signal<string | null>(null);

  ngOnInit(): void {
    this.setupSearch();
    this.loadTransactions();
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  get query(): TransactionHistoryQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search(),
      type: this.type() ?? undefined,
      date: this.date() || undefined
    };
  }
  loadTransactions(): void {
    this.loading.set(true);

    this.stockTransactionService.
      getTransactionHistory(this.query)
      .subscribe({
        next: (response) => {
          this.transactions.set(response.data.items);
          console.log(this.transactions())

          this.page.set(response.data.page);
          this.pageSize.set(response.data.pageSize);
          this.totalItems.set(response.data.totalItems);
          this.totalPages.set(response.data.totalPages);
          this.loading.set(false);
          // this.toast.success()
        },
        error: (err) => {
          // console.log("Full Error",err)
          // console.log("error.error",err.error)
          // console.log("error.error.message",err.error.message)
          console.log(err)
          this.toast.error(err.error?.message ??
            "Unable to load transaction history"
          )
          this.loading.set(false);
        }
      }
      )
  }

  private setupSearch(): void {
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(
      value => {
        this.search.set(value)
        this.page.set(1)
        this.loadTransactions();
      }
    )
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  changeType(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value == "") {
      this.type.set(null);
    } else {
      this.type.set(Number(value));
    }

    this.page.set(1)
    this.loadTransactions();
  }

  changeDate(event: Event): void {
    const value = (event.target as HTMLInputElement).value
    this.date.set(value);
    this.page.set(1)
    this.loadTransactions()
  }
  changePageSize(event: Event): void {
    const size =
      Number(
        (event.target as HTMLSelectElement).value
      );
    this.pageSize.set(size);
    this.page.set(1);
    this.loadTransactions();
  }

  nextPage(): void {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1)
      this.loadTransactions()
    }
  }
  previousPage(): void {
    if (this.page() > 1) {
      this.page.update(
        p => p - 1
      );
      this.loadTransactions();
    }
  }
  goToPage(pageNumber: number): void {
    if (
      pageNumber === this.page() ||
      pageNumber <= 0 ||
      pageNumber > this.totalPages()
    ) {
      return;
    }
    this.page.set(pageNumber);
    this.loadTransactions();
  }

  get pagesList(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      pages.push(i);
    }
    return pages;
  }
  showRemark(remark: string): void {
    this.selectedRemark.set(remark);
  }
}
