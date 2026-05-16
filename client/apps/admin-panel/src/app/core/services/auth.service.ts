import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  data: {
    access_token: string;
    refresh_token: string;
    expires_in: number;
    token_type: string;
    id_token: string;
  };
  statusCode: number;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`;

  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage() {
    const token = localStorage.getItem('access_token');
    if (token) {
      const decoded = this.decodeToken(token);
      this.currentUserSubject.next(decoded);
    }
  }

  private decodeToken(token: string) {
    try {
      const payload = token.split('.')[1];
      const decodedPayload = JSON.parse(atob(payload));
      return {
        token,
        userId: decodedPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decodedPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        roles: decodedPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
        displayName: decodedPayload['DisplayName']
      };
    } catch (e) {
      return null;
    }
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(res => {
        if (res.statusCode === 200 && res.data?.access_token) {
          localStorage.setItem('access_token', res.data.access_token);
          localStorage.setItem('refresh_token', res.data.refresh_token);
          const decoded = this.decodeToken(res.data.access_token);
          this.currentUserSubject.next(decoded);
        }
      })
    );
  }

  getUserRole(): string | string[] {
    return this.currentUserSubject.value?.roles || [];
  }

  logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('access_token');
  }
}
