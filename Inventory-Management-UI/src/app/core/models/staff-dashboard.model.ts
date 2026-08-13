import { DashboardTransaction } from "./dashboard-transaction-model";
import { LowStockProduct } from "./low-stock-product-model";

export interface StaffDashboard {
  totalProducts: number;
  lowStockProducts: number;
  totalStock: number;

  lowStockItems: LowStockProduct[];
  myRecentTransactions: DashboardTransaction[];
}