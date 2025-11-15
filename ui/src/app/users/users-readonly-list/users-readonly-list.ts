import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { UserService, UserSummary } from '../../core/services/user.service';
import { ROLE_NAME_BY_CODE } from '../../core/enums/role.enum';
import { LocalizationService } from '../../core/services/localization.service';

type SortColumn = 'Username' | 'Email' | 'Role' | 'CreatedAt';
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-users-readonly-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './users-readonly-list.html',
  // reuse the same styles as users-list to keep the look identical
  styleUrls: ['../users-list/users-list.scss'],
})
export class UsersReadOnlyListComponent implements OnInit {

  users: UserSummary[] = [];
  errorMessage = '';

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

  constructor(
    private service: UserService,
    public loc: LocalizationService    // 👈 for template
  ) {}

  ngOnInit(): void {
    this.load();
  }

  private normalizeRoleFilter(): string | undefined {
    // option values: All, Admin, User, ReadOnlyUser
    return this.roleFilter === 'All' ? undefined : this.roleFilter;
  }

  load(): void {
    const query = {
      search: this.searchTerm || undefined,
      role: this.normalizeRoleFilter(),
      page: this.page,
      pageSize: this.pageSize,
      sort: this.sortColumn,
      dir: this.sortDir,
    };

    this.service.getUsers(query).subscribe({
      next: res => {
        this.users = res.items || [];
        this.totalItems = res.totalCount;
        this.totalPages = Math.max(1, Math.ceil(this.totalItems / this.pageSize));
      },
      error: err => {
        console.error(err);
        this.errorMessage =
          this.loc.t('Users.Error.LoadFailed') || 'Failed to load users.';
      },
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
    this.load();
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

  getRoleName(roleCode: number) {
    return ROLE_NAME_BY_CODE[String(roleCode)] ?? roleCode;
  }
}
