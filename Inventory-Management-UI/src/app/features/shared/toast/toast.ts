import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ToastService } from "../../../core/services/toast"

@Component({
  selector: "app-toast",
  imports: [CommonModule],
  templateUrl: "./toast.html",
  styleUrl: "./toast.css"
})
export class Toast {
  constructor(
    public toast: ToastService
  ) {}

}