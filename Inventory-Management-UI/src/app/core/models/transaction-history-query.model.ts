import { TransactionType } from "./enums/transaction-type.model";

export interface TransactionHistoryQuery {
    page: number;
    pageSize: number;
    search?: string;
    type?: TransactionType;
    date?: string;
}