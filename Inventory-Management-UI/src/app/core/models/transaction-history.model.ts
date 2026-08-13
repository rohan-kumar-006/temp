import { TransactionType } from "./enums/transaction-type.model";

export interface TransactionHistory {
    id: number;
    productName: string;
    sku: string;
    type: TransactionType;
    quantity: number;
    performedBy: string;
    remarks?: string;
    createdAt: string;
}