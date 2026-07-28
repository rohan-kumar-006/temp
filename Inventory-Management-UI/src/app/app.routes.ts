import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { authGuard } from './core/guards/auth-guard';
import { StaffManagement } from './features/users/staff-management/staff-management';
import { loginGuard } from './core/guards/login-guard';

export const routes: Routes = [
    {
        path: "",
        redirectTo: "login",
        pathMatch: "full"
    },
    {
        path: "login",
        component: Login,
        canActivate:[loginGuard]
    },
    {
        path: 'dashboard',
        component: Dashboard,
        canActivate:[authGuard]
    },
    {
        path: 'users',
        component: StaffManagement,
        canActivate:[authGuard]
    }
];
