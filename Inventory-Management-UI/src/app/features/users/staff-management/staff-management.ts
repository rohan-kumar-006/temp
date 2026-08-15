import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CreateUser } from '../../../core/models/create-user.model';
import { UpdateUser } from '../../../core/models/update-user.model';
import { ToastService } from '../../../core/services/toast';
import { Modal } from 'bootstrap';

@Component({
  selector: 'app-staff-management',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css',
})
export class StaffManagement implements OnInit {

  users = signal<User[]>([]);
  loading = signal(true);
  currentPage = signal(1)
  pageSize = signal(10)
  totalItems = signal(0)
  totalPages = signal(0)
  search = signal("")

  editingUserId = signal<number | null>(null);
  isEditMode = signal(false);

  private fb = inject(FormBuilder)
  private staffModalInstance?: Modal;
  constructor(private userService: UserService, private toast: ToastService) { }

  ngOnInit() {
    this.loadUsers();

    const modalEl = document.getElementById('staffModal');
    if (modalEl) {
      this.staffModalInstance = new Modal(modalEl, { backdrop: true });
      modalEl.addEventListener('hidden.bs.modal', () => {
        this.resetForm();
      });
    }
  }
  ngOnDestroy(): void {
    this.staffModalInstance?.dispose();
  }

  private openStaffModal(): void {
    this.staffModalInstance?.show();
  }

  private closeStaffModal(): void {
    this.staffModalInstance?.hide();
  }

  loadUsers(): void {
    this.loading.set(true);

    this.userService.getAllStaff(this.currentPage(), this.pageSize(), this.search())
      .subscribe({
        next: (response) => {
          this.users.set(response.data.items);
          this.currentPage.set(response.data.page);
          this.pageSize.set(response.data.pageSize);

          this.totalItems.set(response.data.totalItems);
          this.totalPages.set(response.data.totalPages);
          this.loading.set(false);
        },
        error: (err) => {
          this.loading.set(false);

          this.toast.error(
            err.error?.message ??
            "Unable to load staff."
          );
        }
      });
  }

  createStaff() {
    if (this.staffForm.invalid) {
      return;
    }
    this.userService.createStaff(
      this.staffForm.value as CreateUser
    ).subscribe({
      next: () => {
        this.loadUsers();
        this.closeStaffModal();
        this.toast.success("Staff created successfully.");
      },
      error: (err) => {
        this.toast.error(
          err.error?.message ??
          "Unable to create staff."
        );
      }
    })
  }

  staffForm = this.fb.group({
    fullName: [
      '',
      Validators.required
    ],
    email: [
      '',
      [Validators.required,
      Validators.email]
    ],
    password: [
      '',
      [Validators.required,
      Validators.minLength(6)]
    ],
  })

  editUser(user: User) {
    this.staffForm.patchValue({
      fullName: user.fullName,
      email: user.email,
      password: ""
    });

    this.staffForm.get('password')?.clearValidators();
    this.staffForm.get('password')?.updateValueAndValidity();

    this.editingUserId.set(user.id);
    this.isEditMode.set(true);
    this.openStaffModal();
  }

  updateStaff() {
    if (this.staffForm.invalid) {
      return;
    }

    const request: UpdateUser = {
      fullName: this.staffForm.value.fullName!,
      email: this.staffForm.value.email!
    }

    this.userService.updateStaff(this.editingUserId()!, request)
      .subscribe({
        next: (response) => {
          this.users.update(users =>
            users.map(user =>
              user.id === response.data.id ? response.data : user
            ))
          this.toast.success(
            "Staff updated successfully."
          );
          this.closeStaffModal();

        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            "Unable to update staff."
          );
        }
      })
  }
  addStaff() {
    this.resetForm();
    this.openStaffModal();
  }

  saveUser() {
    if (this.isEditMode()) {
      this.updateStaff();
    } else {
      this.createStaff();
    }
  }

  resetForm() {
    this.staffForm.reset();

    this.staffForm.get('password')?.setValidators([
      Validators.required,
      Validators.minLength(6)
    ]);

    this.staffForm.get('password')?.updateValueAndValidity();

    this.isEditMode.set(false);
    this.editingUserId.set(null);
  }

  toggleStatus(user: User) {
    const confirmed = confirm(`Do You want to ${user.isActive ? "Deactivate" : "Activate"} ${user.fullName} `);
    if (!confirmed) return

    this.userService.toggleStatus(user.id)
      .subscribe({
        next: (response) => {
          this.users.update(users =>
            users.map(u =>
              u.id === response.data.id ? response.data : u
            )
          )
          this.toast.success(response.message);
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            "Unable to update status."
          );
        }
      })
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.currentPage.set(page);
    this.loadUsers();
  }
  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(page => page + 1);
      this.loadUsers();
    }
  }
  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(page => page - 1);
      this.loadUsers();
    }
  }
  onSearch(): void {
    this.currentPage.set(1);
    this.loadUsers();
  }
  clearSearch(): void {
    this.search.set('');
    this.currentPage.set(1);
    this.loadUsers();
  }
}