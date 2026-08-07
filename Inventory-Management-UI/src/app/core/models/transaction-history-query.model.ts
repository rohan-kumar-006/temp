export interface TransactionHistoryQuery {
    page: number;
    pageSize: number;
    search?: string;
    type?: number;
    date?: string;
}