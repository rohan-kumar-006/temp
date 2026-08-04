import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateStockTransaction } from "../models/create-stock-transaction.model";
import { ApiResponse } from "../models/app-response.model";
import { StockTransactionResponse } from "../models/stock-transaction-response.model";

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
}