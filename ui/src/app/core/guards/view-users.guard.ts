// src/app/core/guards/view-users.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const viewUsersGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const roles = auth.getUserRoles() || [];

  // ✅ Admin and ReadOnlyUser can see a users list
  if (roles.includes('Admin') || roles.includes('ReadOnlyUser')) {
    return true;
  }

  // Normal User → back to profile
  if (roles.includes('User')) {
    return router.createUrlTree(['/profile']);
  }

  // Fallback
  return router.createUrlTree(['/login']);
};
