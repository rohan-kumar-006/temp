import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CreateUser } from '../../../core/models/create-user.model';

@Component({
  selector: 'app-staff-management',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css',
})
export class StaffManagement implements OnInit {

  users: User[] = [];
  loading = true;
  errorMessage = '';
  successMessage = '';

  private fb = inject(FormBuilder);

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAllStaff().subscribe({
      next: (response) => {

        console.log('Response:', response);

        this.users = response.data;

        console.log('Users after assignment:', this.users);

        this.loading = false;

        // Force Angular to update the UI
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;

        this.cdr.detectChanges();
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

        this.staffForm.reset();

        this.errorMessage = '';

        this.successMessage = 'Staff created successfully.';

        this.cdr.detectChanges();
      },
      error: (err) => {

        this.successMessage = '';

        this.errorMessage = err.error.message;

        this.cdr.detectChanges();
      }
    });
  }

  staffForm = this.fb.group({
    fullName: [
      '',
      Validators.required
    ],
    email: [
      '',
      [Validators.required, Validators.email]
    ],
    password: [
      '',
      [Validators.required, Validators.minLength(6)]
    ],
  });

}