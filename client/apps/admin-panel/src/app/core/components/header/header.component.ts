import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [CommonModule, RouterModule],
    template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm mb-4" *ngIf="authService.isAuthenticated()">
      <div class="container">
        <a class="navbar-brand fw-bold" routerLink="/sponsorship">
          <span class="text-info">Techzu</span>Ichicode
        </a>
        
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-nav-item">
              <a class="nav-link" routerLink="/sponsorship" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">
                <i class="bi bi-list-task me-1"></i> My Requests
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/sponsorship/new" routerLinkActive="active">
                <i class="bi bi-plus-circle me-1"></i> New Request
              </a>
            </li>
          </ul>
          
          <div class="d-flex align-items-center gap-3">
            <span class="text-light opacity-75 small" *ngIf="authService.currentUser$ | async as user">
              <i class="bi bi-person-circle me-1"></i> {{ user.displayName || 'User' }}
            </span>
            <button class="btn btn-outline-danger btn-sm" (click)="onLogout()">
              <i class="bi bi-box-arrow-right me-1"></i> Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  `
})
export class HeaderComponent {
    authService = inject(AuthService);

    onLogout() {
        this.authService.logout();
    }
}
