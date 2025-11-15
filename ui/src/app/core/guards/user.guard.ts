// src/app/core/guards/user.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const userGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const roles = auth.getUserRoles() || [];

  // ✅ Only "User" role allowed
  if (roles.includes('User')) {
    return true;
  }

  // Admin → go to admin users list
  if (roles.includes('Admin')) {
    return router.createUrlTree(['/users']);
  }

  // ReadOnlyUser → go to read-only list
  if (roles.includes('ReadOnlyUser')) {
    return router.createUrlTree(['/users-read']);
  }

  // Fallback
  return router.createUrlTree(['/login']);
};
