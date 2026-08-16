import { UserRole } from "./enums/user-role.model";

export interface LoginResponse{
    accessToken:string;
    expiresAt:string;
    fullName:string;
    email:string;
    role:UserRole;
}