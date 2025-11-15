import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
})
export class DashboardComponent implements OnInit {

  userName: string | null = null;
  roles: string[] = [];

  isAdmin = false;
  isReadOnlyUser = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName();
    this.roles = this.authService.getUserRoles() || [];

    this.isAdmin = this.roles.includes('Admin');
    this.isReadOnlyUser = this.roles.includes('ReadOnlyUser');
    // role "User" = !isAdmin && !isReadOnlyUser
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
