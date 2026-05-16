import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { AuthService } from '../../../core/services/auth.service';
import { SponsorshipRequest, RequestStatus } from '@client/core-shared';
import { Observable, shareReplay } from 'rxjs';

@Component({
  selector: 'app-request-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './request-list.component.html'
})
export class RequestListComponent implements OnInit {
  private sponsorshipService = inject(SponsorshipService);
  private authService = inject(AuthService);
  
  // Using shareReplay(1) to avoid duplicate HTTP calls when using multiple async pipes
  requests$!: Observable<SponsorshipRequest[]>;

  ngOnInit(): void {
    const roles = this.authService.getUserRole();
    const isAdminOrManager = Array.isArray(roles) 
      ? roles.some(r => r === 'SystemAdmin' || r === 'Manager' || r === 'FinanceAdmin')
      : (roles === 'SystemAdmin' || roles === 'Manager' || roles === 'FinanceAdmin');

    if (isAdminOrManager) {
      this.requests$ = this.sponsorshipService.getPendingApprovals().pipe(shareReplay(1));
    } else {
      this.requests$ = this.sponsorshipService.getMyRequests().pipe(shareReplay(1));
    }
  }

  getFileName(url: string): string {
    if (!url) return '';
    const parts = url.split('/');
    return parts[parts.length - 1];
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Approved': return 'bg-success';
      case 'Rejected': return 'bg-danger';
      case 'PendingManagerApproval':
      case 'PendingFinanceReview': return 'bg-warning text-dark';
      case 'Draft': return 'bg-secondary';
      case 'Cancelled': return 'bg-dark';
      default: return 'bg-light text-dark';
    }
  }
}
