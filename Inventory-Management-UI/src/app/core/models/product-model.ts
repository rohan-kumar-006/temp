export interface Product {
    id: number;
    name: string;
    sku: string;
    description: string;
    price: number;
    quantity: number;
    reorderLevel: number;
    imageUrl?: string;
    createdBy: string;
    createdAt: string;
}