import { TransactionType } from "./enums/transaction-type.model"

export interface CreateStockTransaction{
    productId:number,
    type:TransactionType
    quantity:number,
    remarks?:string
}