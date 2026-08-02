export interface CreateProduct {
    name: string;
    sku: string;
    description: string;
    price: number;
    initialQuantity: number;
    reorderLevel: number;
    image?: File; 
}