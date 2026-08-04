import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CreateUser } from '../../../core/models/create-user.model';
import { UpdateUser } from '../../../core/models/update-user.model';
import { ToastService } from '../../../core/services/toast';
// import { Modal } from 'bootstrap';

@Component({
  selector: 'app-staff-management',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css',
})
export class StaffManagement implements OnInit {

  users = signal<User[]>([]);
  loading = signal(true);

  editingUserId = signal<number | null>(null);
  isEditMode = signal(false);

  private fb = inject(FormBuilder)
  constructor(private userService: UserService, private toast: ToastService) { }

  ngOnInit() {
    // console.log("Content loaded in the onint")
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAllStaff().subscribe({
      next: (response) => {

        this.users.set(response.data);

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


        this.resetForm();
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
          this.staffForm.reset()
          this.isEditMode.set(false)
          this.editingUserId.set(null)

        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            "Unable to update staff."
          );
        }
      })
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
}