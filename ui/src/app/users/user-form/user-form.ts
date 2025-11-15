import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import {
  UserService,
  CreateUserPayload,
  UpdateUserPayload,
  UserSummary
} from '../../core/services/user.service';

import { ROLE_SELECT_OPTIONS } from '../../core/enums/role.enum';
import { LocalizationService } from '../../core/services/localization.service';
import { extractApiError } from '../../core/utils/error.util';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.html',
  styleUrls: ['./user-form.scss'],
})
export class UserFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  userId: number | null = null;

  isSubmitting = false;
  errorMessage = '';

  roleOptions = ROLE_SELECT_OPTIONS;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    public loc: LocalizationService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.userId = +idParam;
    }

    this.buildForm();

    if (this.isEdit && this.userId) {
      this.loadUser(this.userId);
    }
  }

  // ----------------------------------------------------------
  // Build Form
  // ----------------------------------------------------------
  private buildForm(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      role: ['2', Validators.required],
      password: [''] // required only for CREATE
    });

    if (!this.isEdit) {
      this.form.get('password')?.setValidators([
        Validators.required,
        Validators.minLength(8)
      ]);
    }
  }

  // ----------------------------------------------------------
  // Load User (Edit Mode)
  // ----------------------------------------------------------
  private loadUser(id: number): void {
    this.userService.getUser(id).subscribe({
      next: (user: UserSummary) => {
        this.form.patchValue({
          username: user.username ?? '',
          email: user.email ?? '',
          role: user.role?.toString() ?? '2',
          password: ''
        });
      },
      error: (err) => {
        console.error('Load user error', err);
        this.errorMessage =
          extractApiError(err) ||
          this.loc.t('UserForm.Error.LoadUser') ||
          'Failed to load user.';
      }
    });
  }

  // ----------------------------------------------------------
  // Submit Form
  // ----------------------------------------------------------
  submit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    const roleCode = Number(raw.role);

    // ------------------------------------------------------
    // CREATE USER
    // ------------------------------------------------------
    if (!this.isEdit) {
      const payload: CreateUserPayload = {
        username: raw.username.trim(),
        email: raw.email.trim(),
        password: raw.password,
        role: roleCode
      };

      this.userService.createUser(payload).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['/users']);
        },
        error: (err) => {
          console.error('Create user error', err);
          this.isSubmitting = false;
          this.errorMessage =
            extractApiError(err) ||
            this.loc.t('UserForm.Error.CreateUser') ||
            'Failed to create user.';
        }
      });

      return;
    }

    // ------------------------------------------------------
    // UPDATE USER
    // ------------------------------------------------------
    if (this.userId !== null) {
      const payload: UpdateUserPayload = {
        username: raw.username.trim(),
        email: raw.email.trim(),
        role: roleCode,
        newPassword: raw.password ? raw.password : null
      };

      this.userService.updateUser(this.userId, payload).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['/users']);
        },
        error: (err) => {
          console.error('Update user error', err);
          this.isSubmitting = false;
          this.errorMessage =
            extractApiError(err) ||
            this.loc.t('UserForm.Error.UpdateUser') ||
            'Failed to update user.';
        }
      });
    }
  }

  // ----------------------------------------------------------
  // Cancel
  // ----------------------------------------------------------
  cancel(): void {
    this.router.navigate(['/users']);
  }

  // ----------------------------------------------------------
  // Title helper (localized)
  // ----------------------------------------------------------
  get title(): string {
    return this.isEdit
      ? this.loc.t('UserForm.Title.Edit')
      : this.loc.t('UserForm.Title.Create');
  }
}
