import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

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

export interface UserListQuery {
  search?: string;
  role?: string;
  page?: number;
  pageSize?: number;
  sort?: string;
  dir?: 'asc' | 'desc';
}

// 🔹 DataTable response shape from /api/v1/users/dt
export interface DataTableResponse<T> {
  draw: number;
  recordsTotal: number;
  recordsFiltered: number;
  data: T[];
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = 'http://localhost:5177/api/v1/users';

  constructor(private http: HttpClient) {}

  // ✅ original list endpoint (GET /api/v1/users)
  getUsers(query: UserListQuery): Observable<PagedResult<UserSummary>> {
    let params = new HttpParams();

    if (query.search) params = params.set('search', query.search);
    if (query.role) params = params.set('role', query.role);
    if (query.page) params = params.set('page', query.page.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.sort) params = params.set('sort', query.sort);
    if (query.dir) params = params.set('dir', query.dir);

    return this.http.get<PagedResult<UserSummary>>(this.baseUrl, { params });
  }

  // ✅ DataTable server-side endpoint (POST /api/v1/users/dt)
  getUsersDataTable(dtReq: any, role?: string): Observable<DataTableResponse<UserSummary>> {
    let params = new HttpParams();

    // role is enum on backend (UserRole?),
    // here we pass string name: 'Admin' | 'User' | 'ReadOnlyUser'
    if (role && role !== 'All') {
      params = params.set('role', role);
    }

    return this.http.post<DataTableResponse<UserSummary>>(
      `${this.baseUrl}/dt`,
      dtReq,
      { params }
    );
  }

  getUser(id: number): Observable<UserSummary> {
    return this.http.get<UserSummary>(`${this.baseUrl}/${id}`);
  }

  createUser(payload: CreateUserPayload): Observable<UserSummary> {
    return this.http.post<UserSummary>(this.baseUrl, payload);
  }

  updateUser(id: number, payload: UpdateUserPayload): Observable<UserSummary> {
    return this.http.put<UserSummary>(`${this.baseUrl}/${id}`, payload);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }



  getUserProfile(): Observable<UserSummary> {
  return this.http.get<UserSummary>(`${this.baseUrl}/profile`);
}
updateUserProfile(payload: UpdateProfilePayload): Observable<UserSummary> {
  return this.http.put<UserSummary>(`${this.baseUrl}/profile`, payload);
}

}
