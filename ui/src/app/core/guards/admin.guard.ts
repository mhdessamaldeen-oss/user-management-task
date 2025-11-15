// src/app/core/guards/admin.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const roles = auth.getUserRoles() || [];

  // ✅ Admin allowed
  if (roles.includes('Admin')) {
    return true;
  }

  // ReadOnlyUser → send to read-only users list
  if (roles.includes('ReadOnlyUser')) {
    return router.createUrlTree(['/users-read']);
  }

  // Normal User → send to profile
  if (roles.includes('User')) {
    return router.createUrlTree(['/profile']);
  }

  // Fallback
  return router.createUrlTree(['/login']);
};
