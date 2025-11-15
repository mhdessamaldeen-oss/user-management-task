import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { LocalizationService } from '../core/services/localization.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './main-layout.html',
  styleUrls: ['./main-layout.scss'],
})
export class MainLayoutComponent implements OnInit {

  userName: string | null = null;
  roles: string[] = [];

  isAdmin = false;
  isReadOnlyUser = false;
  isUser = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    public loc: LocalizationService   // 👈 public for template
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName();
    this.roles = this.authService.getUserRoles() || [];

    this.isAdmin = this.roles.includes('Admin');
    this.isReadOnlyUser = this.roles.includes('ReadOnlyUser');
    this.isUser = this.roles.includes('User');
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  changeLang(lang: 'en' | 'ar'): void {
    this.loc.load(lang).subscribe();
  }
}
