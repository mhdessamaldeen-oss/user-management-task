// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../config/api.config';
import { ROLE_NAME_BY_CODE } from '../enums/role.enum';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {

  constructor(private http: HttpClient) {}

  login(model: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      API_BASE_URL + API_ENDPOINTS.auth.login,
      model
    ).pipe(
      tap(res => {
        localStorage.setItem('access_token', res.token);
        localStorage.setItem('token_expires_at', res.expiresAt);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('token_expires_at');
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getUserName(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['unique_name'] || null;
    } catch {
      return null;
    }
  }

  getUserRoles(): string[] {
    const token = this.getToken();
    if (!token) return [];

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      let roles: any = payload['role'] || payload['roles'];
      const msClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
      if (!roles && payload[msClaim]) roles = payload[msClaim];

      if (!roles) return [];

      const rawRoles: string[] = Array.isArray(roles) ? roles : [roles];
      return rawRoles.map(code => ROLE_NAME_BY_CODE[code] ?? code);

    } catch {
      return [];
    }
  }
}
