export interface ProductQuery {
    page: number;
    pageSize: number;
    search?: string;
    minPrice?: number;
    maxPrice?: number;
    lowStockOnly?: boolean;
    sortBy?: string;
    descending?: boolean;
}