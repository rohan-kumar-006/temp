export interface TransactionHistory {
    id: number;
    productName: string;
    sku: string;
    type: number;
    quantity: number;
    performedBy: string;
    remarks?: string;
    createdAt: string;
}