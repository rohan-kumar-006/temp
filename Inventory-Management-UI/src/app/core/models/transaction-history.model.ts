export interface TransactionHistory {
    id: number;
    productName: string;
    sku: string;
    type: string;
    quantity: number;
    performedBy: string;
    remarks?: string;
    createdAt: string;
}