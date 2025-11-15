import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { UserService, UserSummary } from '../../core/services/user.service';
import { AuthService } from '../../core/services/auth.service';
import { ROLE_NAME_BY_CODE } from '../../core/enums/role.enum';
import { LocalizationService } from '../../core/services/localization.service';

type SortColumn = 'Username' | 'Email' | 'Role' | 'CreatedAt';
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './users-list.html',
  styleUrls: ['./users-list.scss']
})
export class UsersListComponent implements OnInit {

  users: UserSummary[] = [];
  errorMessage = '';
  isAdmin = false;

  // filters
  searchTerm = '';
  roleFilter = 'All';

  // paging
  page = 1;
  pageSize = 10;
  totalPages = 1;
  totalItems = 0;

  // sorting
  sortColumn: SortColumn = 'Username';
  sortDir: SortDir = 'asc';

  // map DataTable column ordering
  private readonly sortColumnsOrder: SortColumn[] = [
    'Username',
    'Email',
    'Role',
    'CreatedAt'
  ];

  constructor(
    private service: UserService,
    private auth: AuthService,
    public loc: LocalizationService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.auth.getUserRoles().includes('Admin');
    this.load();
  }

  // Convert UI role filter → backend role filter
  private normalizeRoleFilter(): string | undefined {
    // option values are: All, Admin, User, ReadOnlyUser
    return this.roleFilter === 'All' ? undefined : this.roleFilter;
  }

  // Build DataTable-style request
  private buildDataTableRequest() {
    const columnIndex = this.sortColumnsOrder.indexOf(this.sortColumn);

    return {
      draw: this.page,
      start: (this.page - 1) * this.pageSize,
      length: this.pageSize,
      search: {
        value: this.searchTerm || '',
        regex: false
      },
      order: [
        {
          column: columnIndex,
          dir: this.sortDir
        }
      ],
      columns: this.sortColumnsOrder.map(col => ({
        data: col,
        name: col,
        searchable: true,
        orderable: true,
        search: { value: '', regex: false }
      }))
    };
  }

  load(): void {
    const dtReq = this.buildDataTableRequest();
    const role = this.normalizeRoleFilter();

    this.service.getUsersDataTable(dtReq, role).subscribe({
      next: res => {
        this.users = res.data || [];
        this.totalItems = res.recordsFiltered ?? res.recordsTotal ?? this.users.length;
        this.totalPages = Math.max(1, Math.ceil(this.totalItems / this.pageSize));
      },
      error: err => {
        console.error(err);
        this.errorMessage = this.loc.t('Users.Error.LoadFailed') || 'Failed to load users.';
      }
    });
  }

  reload(): void {
    this.page = 1;
    this.load();
  }

  sort(col: SortColumn) {
    if (this.sortColumn === col) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = col;
      this.sortDir = 'asc';
    }
    this.reload();
  }

  sortIcon(col: SortColumn) {
    if (this.sortColumn !== col) return '';
    return this.sortDir === 'asc'
      ? 'bi bi-caret-up-fill'
      : 'bi bi-caret-down-fill';
  }

  changePage(p: number) {
    if (p < 1 || p > this.totalPages) return;
    this.page = p;
    this.load();
  }

  deleteUser(u: UserSummary) {
    if (!this.isAdmin) return;
    if (!confirm(`Delete user "${u.username}"?`)) return;

    this.service.deleteUser(u.id).subscribe({
      next: () => this.load(),
      error: () => alert(this.loc.t('Users.Error.DeleteFailed') || 'Failed to delete user.')
    });
  }

  getRoleName(roleCode: number | string) {
    return ROLE_NAME_BY_CODE[String(roleCode)] ?? roleCode;
  }
}
