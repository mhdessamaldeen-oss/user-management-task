import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class LoginComponent {

  model: LoginRequest = {
    username: '',
    password: ''
  };

  isLoading = false;
  errorMessage = '';
debugToken: string | null = null;
  currentYear = new Date().getFullYear();   
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  submit() {
  if (!this.model.username || !this.model.password) {
    this.errorMessage = 'Username and password are required.';
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;
  

  this.authService.login(this.model).subscribe({
    next: () => {
      this.isLoading = false;
      this.router.navigate(['/users']);
    },
    error: () => {
      this.isLoading = false;
      this.errorMessage = 'Invalid username or password.';
    }
  });
}


}
