// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../config/api.config';
import { ROLE_NAME_BY_CODE } from '../enums/role.enum';

export interface LoginRequest {
  username: string;   // use 'email' here if your API expects email
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string; // adjust to your API (ISO string etc.)
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly baseUrl = API_BASE_URL;

  constructor(private http: HttpClient) {}

  login(model: LoginRequest): Observable<LoginResponse> {
    const url = this.baseUrl + API_ENDPOINTS.auth.login;

    return this.http.post<LoginResponse>(url, model).pipe(
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

    // 1) Typical 'role' / 'roles'
    let roles: any = payload['role'] || payload['roles'];

    // 2) ASP.NET full claim type for role (your current token)
    const msRoleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    if (!roles && payload[msRoleClaim]) {
      roles = payload[msRoleClaim];
    }

    if (!roles) return [];

    const rawRoles: string[] = Array.isArray(roles) ? roles : [roles];

    // 👇 map "1" → "Admin", "2" → "User", etc.
    return rawRoles.map(code => ROLE_NAME_BY_CODE[code] ?? code);
  } catch {
    return [];
  }
}


  
}
