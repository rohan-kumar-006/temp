import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { CreateStockTransaction } from "../models/create-stock-transaction.model";
import { ApiResponse } from "../models/app-response.model";
import { StockTransactionResponse } from "../models/stock-transaction-response.model";
import { PagedResult } from "../models/paged-result-model";
import { TransactionHistoryQuery } from "../models/transaction-history-query.model";
import { TransactionHistory } from "../models/transaction-history.model";

@Injectable({
    providedIn: "root"
})
export class StockTransactionService {
    private apiUrl = `${environment.apiUrl}/StockTransactions`

    constructor(private http: HttpClient) { }

    createTransaction(request: CreateStockTransaction) {
        return this.http.post<ApiResponse<StockTransactionResponse>>(
            this.apiUrl,
            request
        );
    }
    getTransactionHistory(query: TransactionHistoryQuery) {

        let params = new HttpParams()
            .set('page', query.page)
            .set('pageSize', query.pageSize);

        if (query.search) {
            params = params.set(
                'search',
                query.search
            );
        }

        if (query.type != null) {
            params = params.set(
                'type',
                query.type
            );
        }

        if (query.date) {
            params = params.set(
                'date',
                query.date
            );
        }

        return this.http.get<ApiResponse<PagedResult<TransactionHistory>>>(
            `${this.apiUrl}`,
            { params }
        );

    }
}