export interface CreateStockTransaction{
    productId:number,
    type:"In" | "Out",
    quantity:number,
    remarks?:string
}