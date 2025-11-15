// src/app/core/services/user.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL, API_ENDPOINTS } from '../config/api.config';

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface UserSummary {
  id: number;
  username: string;
  email: string;
  role: number;
  isDeleted?: boolean;
  createdAt?: string;
  updatedAt?: string | null;
}

export interface CreateUserPayload {
  username: string;
  email: string;
  password: string;
  role: number;
}

export interface UpdateUserPayload {
  username: string;
  email: string;
  role: number;
  newPassword?: string | null;
}

export interface UpdateProfilePayload {
  email: string;
  newPassword?: string | null;
}

export interface DataTableResponse<T> {
  draw: number;
  recordsTotal: number;
  recordsFiltered: number;
  data: T[];
}

@Injectable({ providedIn: 'root' })
export class UserService {

  constructor(private http: HttpClient) {}

  // GET users
  getUsers(query: any): Observable<PagedResult<UserSummary>> {
    let params = new HttpParams();

    Object.keys(query).forEach(key => {
      if (query[key] !== undefined && query[key] !== null) {
        params = params.set(key, query[key]);
      }
    });

    return this.http.get<PagedResult<UserSummary>>(
      API_BASE_URL + API_ENDPOINTS.users.list,
      { params }
    );
  }

  // DataTable
  getUsersDataTable(dtReq: any, role?: string): Observable<DataTableResponse<UserSummary>> {
    let params = new HttpParams();

    if (role && role !== 'All') params = params.set('role', role);

    return this.http.post<DataTableResponse<UserSummary>>(
      API_BASE_URL + API_ENDPOINTS.users.dt,
      dtReq,
      { params }
    );
  }

  getUser(id: number): Observable<UserSummary> {
    return this.http.get<UserSummary>(
      API_BASE_URL + API_ENDPOINTS.users.byId(id)
    );
  }

  createUser(payload: CreateUserPayload): Observable<UserSummary> {
    return this.http.post<UserSummary>(
      API_BASE_URL + API_ENDPOINTS.users.create,
      payload
    );
  }

  updateUser(id: number, payload: UpdateUserPayload): Observable<UserSummary> {
    return this.http.put<UserSummary>(
      API_BASE_URL + API_ENDPOINTS.users.update(id),
      payload
    );
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(
      API_BASE_URL + API_ENDPOINTS.users.delete(id)
    );
  }

  getUserProfile(): Observable<UserSummary> {
    return this.http.get<UserSummary>(
      API_BASE_URL + '/users/profile'
    );
  }

  updateUserProfile(payload: UpdateProfilePayload): Observable<UserSummary> {
    return this.http.put<UserSummary>(
      API_BASE_URL + '/users/profile',
      payload
    );
  }
}
