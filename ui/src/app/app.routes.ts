import { Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login';
import { MainLayoutComponent } from './layout/main-layout';

import { UsersListComponent } from './users/users-list/users-list';
import { UsersReadOnlyListComponent } from './users/users-readonly-list/users-readonly-list';
import { UserFormComponent } from './users/user-form/user-form';
import { UserProfileComponent } from './users/user-profile/user-profile';

import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { viewUsersGuard } from './core/guards/view-users.guard';
import { userGuard } from './core/guards/user.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'profile' },

      { path: 'users', component: UsersListComponent, canActivate: [adminGuard] },
      { path: 'users-read', component: UsersReadOnlyListComponent, canActivate: [viewUsersGuard] },

      { path: 'users/new', component: UserFormComponent, canActivate: [adminGuard] },
      { path: 'users/:id/edit', component: UserFormComponent, canActivate: [adminGuard] },

      { path: 'profile', component: UserProfileComponent, canActivate: [userGuard] }
    ]
  },

  { path: '**', redirectTo: '' }
];
