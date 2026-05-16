import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    template: `
    <div class="container-fluid min-vh-100 d-flex align-items-center justify-content-center bg-dark">
      <div class="card bg-secondary text-white shadow-lg p-4 rounded-4" style="max-width: 400px; width: 100%;">
        <div class="text-center mb-4">
          <div class="d-inline-block bg-primary text-white p-2 rounded-3 mb-3">
             <i class="bi bi-shield-lock-fill fs-3"></i>
          </div>
          <h2 class="fw-bold">Techzu Ichicode</h2>
          <p class="text-white-50">Sign in to manage sponsorship</p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label class="form-label small fw-bold">Email Address</label>
            <div class="input-group">
              <span class="input-group-text bg-dark text-white-50">
                <i class="bi bi-envelope"></i>
              </span>
              <input 
                type="text" 
                class="form-control bg-dark text-white" 
                formControlName="username" 
                placeholder="email@techzu.com"
                [class.is-invalid]="isFieldInvalid('username')"
              >
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label small fw-bold">Password</label>
            <div class="input-group">
              <span class="input-group-text bg-dark text-white-50">
                <i class="bi bi-lock"></i>
              </span>
              <input 
                type="password" 
                class="form-control bg-dark text-white" 
                formControlName="password" 
                placeholder="••••••••"
                [class.is-invalid]="isFieldInvalid('password')"
              >
            </div>
          </div>

          <div class="d-flex justify-content-between align-items-center mb-4">
            <div class="form-check small">
              <input class="form-check-input" type="checkbox" id="rememberMe" formControlName="rememberMe">
              <label class="form-check-label text-white-50" for="rememberMe">Remember me</label>
            </div>
            <a href="#" class="text-info text-decoration-none small">Forgot?</a>
          </div>

          <button type="submit" class="btn btn-info w-100 py-2 fw-bold" [disabled]="loginForm.invalid || isLoading">
            <span *ngIf="!isLoading">Sign In</span>
            <span *ngIf="isLoading" class="spinner-border spinner-border-sm me-2"></span>
            <span *ngIf="isLoading">Authenticating...</span>
          </button>

          <div *ngIf="errorMessage" class="alert alert-danger mt-3 py-2 small bg-danger bg-opacity-25 text-danger">
            <i class="bi bi-exclamation-triangle-fill me-2"></i> {{ errorMessage }}
          </div>
        </form>

        <div class="text-center mt-4 pt-2 border-top border-dark">
          <p class="text-white-50 small mb-0">Don't have an account? <a href="#" class="text-info text-decoration-none">Contact Admin</a></p>
        </div>
      </div>
    </div>
  `,
    styles: [`
    :host { display: block; }
    .card { background-color: #2b3035 !important; }
    .form-control::placeholder { color: rgba(255,255,255,0.2) !important; }
    .bg-dark { background-color: #1a1d20 !important; }
  `]
})
export class LoginComponent implements OnInit {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);

    loginForm = this.fb.group({
        username: ['', [Validators.required]],
        password: ['', [Validators.required]],
        rememberMe: [false]
    });

    isLoading = false;
    errorMessage = '';

    ngOnInit(): void {
        if (this.authService.isAuthenticated()) {
            this.router.navigate(['/sponsorship']);
        }
    }

    onSubmit() {
        if (this.loginForm.valid) {
            this.isLoading = true;
            this.errorMessage = '';

            this.authService.login(this.loginForm.value as any).subscribe({
                next: (res) => {
                    this.isLoading = false;
                    if (res.statusCode === 200) {
                        this.router.navigate(['/sponsorship']);
                    } else {
                        this.errorMessage = res.message || 'Login failed. Please check your credentials.';
                    }
                },
                error: (err) => {
                    this.isLoading = false;
                    this.errorMessage = err.error?.message || 'An error occurred during login. Please try again later.';
                }
            });
        }
    }

    isFieldInvalid(field: string): boolean {
        const control = this.loginForm.get(field);
        return !!(control && control.invalid && (control.dirty || control.touched));
    }
}
