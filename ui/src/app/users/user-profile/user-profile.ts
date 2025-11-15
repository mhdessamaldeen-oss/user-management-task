import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import {
  UserService,
  UserSummary,
  UpdateProfilePayload
} from '../../core/services/user.service';

import { LocalizationService } from '../../core/services/localization.service';
import { extractApiError } from '../../core/utils/error.util';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-profile.html',
  styleUrls: ['./user-profile.scss'],
})
export class UserProfileComponent implements OnInit {

  user: UserSummary | null = null;

  email = '';
  newPassword = '';
  confirmPassword = '';

  isSaving = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private userService: UserService,
    public loc: LocalizationService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  // ----------------------------------------------------------
  // Load Profile
  // ----------------------------------------------------------
  loadProfile(): void {
    this.errorMessage = '';

    this.userService.getUserProfile().subscribe({
      next: u => {
        this.user = u;
        this.email = u.email;
      },
      error: err => {
        console.error('Load profile error', err);
        this.errorMessage =
          extractApiError(err) ||
          this.loc.t('UserProfile.Error.LoadProfile') ||
          'Failed to load profile.';
      }
    });
  }

  // ----------------------------------------------------------
  // Save Profile
  // ----------------------------------------------------------
  save(): void {
    this.errorMessage = '';
    this.successMessage = '';

    // Validation - Email required
    if (!this.email) {
      this.errorMessage =
        this.loc.t('UserProfile.Validation.EmailRequired') ||
        'Email is required.';
      return;
    }

    // Validation - Passwords must match
    if (this.newPassword && this.newPassword !== this.confirmPassword) {
      this.errorMessage =
        this.loc.t('UserProfile.Validation.PasswordsMismatch') ||
        'Passwords do not match.';
      return;
    }

    const payload: UpdateProfilePayload = {
      email: this.email.trim(),
      newPassword: this.newPassword || null
    };

    this.isSaving = true;

    this.userService.updateUserProfile(payload).subscribe({
      next: _ => {
        this.isSaving = false;

        // clear fields
        this.newPassword = '';
        this.confirmPassword = '';

        this.successMessage =
          this.loc.t('UserProfile.Success.ProfileUpdated') ||
          'Profile updated successfully.';
      },
      error: err => {
        console.error('Update profile error', err);
        this.isSaving = false;

        this.errorMessage =
          extractApiError(err) ||
          this.loc.t('UserProfile.Error.UpdateProfile') ||
          'Failed to update profile.';
      }
    });
  }
}
