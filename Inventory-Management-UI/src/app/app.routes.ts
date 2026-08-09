import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { StaffManagement } from './features/users/staff-management/staff-management';
import { ProductList } from './features/products/product-list/product-list';
import { StockManagement } from './features/stock/stock-management/stock-management';
import { Layout } from './layout/layout';

import { authGuard } from './core/guards/auth-guard';
import { loginGuard } from './core/guards/login-guard';
import { adminGuard } from './core/guards/admin-guard';
import { TransactionHistoryComponent } from './features/transaction-history/transaction-history';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: Login,
    canActivate: [loginGuard]
  },

  {
    path: '',
    component: Layout,
    canActivateChild: [authGuard],

    children: [

      {
        path: 'dashboard',
        component: Dashboard
      },

      {
        path: 'users',
        component: StaffManagement,
        canActivate: [adminGuard]
      },

      {
        path: 'products',
        component: ProductList
      },

      {
        path: 'stock-management',
        component: StockManagement
      },
      {
        path: 'transaction-history',
        component: TransactionHistoryComponent,
        canActivate: [adminGuard]
      }
    ]
  }

];