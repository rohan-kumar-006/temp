import { UserRole } from "./enums/user-role.model";

export interface User{
    id:number,
    fullName:string,
    email:string,
    role:UserRole,
    isActive:boolean
}