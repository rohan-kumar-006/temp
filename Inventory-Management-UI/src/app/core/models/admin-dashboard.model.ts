import { DashboardTransaction } from "./dashboard-transaction-model";
import { LowStockProduct } from "./low-stock-product-model";

export interface AdminDashboard {
  totalProducts: number;
  lowStockProducts: number;
  totalStaff: number;
  totalStock: number;
  stockInToday: number;
  stockOutToday: number;
  transactionsToday: number;

  lowStockItems: LowStockProduct[];
  recentTransactions: DashboardTransaction[];
}