import { TransactionType } from "./enums/transaction-type.model";

export interface DashboardTransaction {
    id: number,
    productName: string,
    sku: string,
    type: TransactionType,
    quantity: number;
    performedBy?: string;
    createdAt: string;
}