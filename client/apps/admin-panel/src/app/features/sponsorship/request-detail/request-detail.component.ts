import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { AuthService } from '../../../core/services/auth.service';
import { SponsorshipRequest, WorkflowHistory, RequestStatus } from '@client/core-shared';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-request-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="container py-5">
      <div class="card shadow-lg border-0 rounded-4 overflow-hidden">
        <div class="card-header bg-primary text-white p-4 d-flex justify-content-between align-items-center">
          <h3 class="mb-0 fw-bold">Request #{{ id?.substring(0,8) }}</h3>
          <button class="btn btn-light btn-sm rounded-pill px-3" routerLink="/sponsorship">
            <i class="bi bi-arrow-left me-2"></i>Back to List
          </button>
        </div>

        <div class="card-body p-4 p-md-5">
          <div *ngIf="isLoading" class="text-center py-5">
            <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status"></div>
            <p class="mt-3 text-secondary fs-5">Loading data...</p>
          </div>

          <div *ngIf="error && !isLoading" class="alert alert-danger p-4 rounded-4 shadow-sm">
            <p class="mb-0">{{ error }}</p>
            <button class="btn btn-outline-danger mt-3" (click)="loadData()">Retry Connection</button>
          </div>

          <div *ngIf="request && !isLoading">
            <div class="row g-4">
              <div class="col-lg-8">
                <!-- Status Badge -->
                <div class="mb-3">
                   <span class="badge rounded-pill px-4 py-2 fs-6 shadow-sm" [ngClass]="getStatusClass(request.status)">
                      {{ request.status }}
                   </span>
                </div>

                <div class="mb-4">
                  <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Sponsorship Title</label>
                  <h2 class="fw-bold text-dark">{{ request.title }}</h2>
                </div>

                <div class="row g-4 mb-4">
                  <div class="col-sm-6">
                    <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Event Name</label>
                    <p class="fs-5 text-dark fw-medium">{{ request.eventName }}</p>
                  </div>
                  <div class="col-sm-6">
                    <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Department</label>
                    <p class="fs-5 text-dark fw-medium">{{ request.department }}</p>
                  </div>
                  <div class="col-sm-6">
                    <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Requested Amount</label>
                    <p class="fs-3 fw-bold text-primary">{{ request.requestedAmount | currency }}</p>
                  </div>
                  <div class="col-sm-6">
                    <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Event Date</label>
                    <p class="fs-5 text-dark fw-medium">{{ request.eventDate | date:'longDate' }}</p>
                  </div>
                </div>

                <div class="p-4 bg-light rounded-4 mb-4 border-start border-4 border-primary">
                  <label class="text-muted small text-uppercase fw-bold mb-2 d-block">Purpose & Business Benefit</label>
                  <p class="mb-0 text-dark" style="white-space: pre-wrap;">{{ request.purpose }}</p>
                  <div *ngIf="request.expectedBusinessBenefit" class="mt-3 pt-3 border-top">
                     <p class="mb-0 text-secondary">{{ request.expectedBusinessBenefit }}</p>
                  </div>
                </div>

                <div *ngIf="request.documentUrl" class="alert alert-info border-0 rounded-4 d-flex align-items-center p-3 mb-4 shadow-sm">
                   <i class="bi bi-file-earmark-check fs-2 me-3 text-primary"></i>
                   <div class="flex-grow-1">
                      <h6 class="mb-0 fw-bold">Supporting Document Attached</h6>
                      <small class="text-secondary">Official attachment for this request</small>
                   </div>
                   <a [routerLink]="['/preview']" [queryParams]="{ file: getFileName(request.documentUrl) }" target="_blank" class="btn btn-primary rounded-pill px-4 shadow-sm">
                      View File
                   </a>
                </div>

                <!-- Action Section -->
                <div *ngIf="canTakeAction()" class="mt-5 p-4 border rounded-4 bg-white shadow-sm">
                   <h4 class="fw-bold mb-3">Workflow Action</h4>
                   <div class="mb-3">
                      <label class="form-label small fw-bold">Remarks / Comments (Optional)</label>
                      <textarea class="form-control rounded-3" [(ngModel)]="remarks" rows="3" placeholder="Enter your comments here..."></textarea>
                   </div>
                   <div class="d-flex flex-wrap gap-2">
                      <ng-container *ngIf="userRole === 'Manager' && request.status === 'PendingManagerApproval'">
                         <button class="btn btn-success px-4 rounded-pill" (click)="transition('PendingFinanceReview')">Approve & Forward</button>
                         <button class="btn btn-warning px-4 rounded-pill" (click)="transition('NeedClarification')">Need Clarification</button>
                         <button class="btn btn-danger px-4 rounded-pill" (click)="transition('Rejected')">Reject</button>
                      </ng-container>

                      <ng-container *ngIf="userRole === 'FinanceAdmin' && request.status === 'PendingFinanceReview'">
                         <button class="btn btn-success px-4 rounded-pill" (click)="transition('Approved')">Final Approve</button>
                         <button class="btn btn-warning px-4 rounded-pill" (click)="transition('NeedClarification')">Need Clarification</button>
                         <button class="btn btn-danger px-4 rounded-pill" (click)="transition('Rejected')">Reject</button>
                      </ng-container>

                      <ng-container *ngIf="userRole === 'Requestor' && (request.status === 'Draft' || request.status === 'NeedClarification')">
                         <button class="btn btn-primary px-4 rounded-pill" (click)="transition('PendingManagerApproval')">Submit for Approval</button>
                         <button class="btn btn-outline-danger px-4 rounded-pill" (click)="transition('Cancelled')">Cancel Request</button>
                      </ng-container>
                      
                      <ng-container *ngIf="userRole === 'SystemAdmin'">
                         <button class="btn btn-outline-secondary px-4 rounded-pill" (click)="transition('Draft')">Reset to Draft (Admin)</button>
                      </ng-container>
                   </div>
                </div>
              </div>

              <div class="col-lg-4">
                <div class="p-4 bg-light rounded-4 h-100 shadow-sm border">
                  <h5 class="fw-bold mb-4 border-bottom pb-2"><i class="bi bi-clock-history me-2"></i>Status Timeline</h5>
                  <div class="list-group list-group-flush bg-transparent">
                    <div class="list-group-item bg-transparent border-0 ps-0 py-3" *ngFor="let log of history">
                       <div class="d-flex align-items-center mb-1">
                          <span class="badge rounded-pill bg-primary me-2">{{ log.toStatus }}</span>
                          <small class="text-muted ms-auto">{{ log.actionDate | date:'short' }}</small>
                       </div>
                       <div class="small fw-bold">{{ log.actionByName }}</div>
                       <div class="small text-secondary fst-italic" *ngIf="log.remarks">"{{ log.remarks }}"</div>
                    </div>
                    <div *ngIf="!history.length" class="text-center py-4 text-secondary">
                       <i class="bi bi-info-circle d-block mb-1"></i>
                       No history records yet.
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RequestDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sponsorshipService = inject(SponsorshipService);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);

  request?: SponsorshipRequest;
  history: WorkflowHistory[] = [];
  id: string | null = null;
  userRole: string | null = null;
  isLoading = true;
  error: string | null = null;
  remarks: string = '';

  ngOnInit(): void {
    this.userRole = this.authService.getUserRole() as string;
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadData();
    }
  }

  loadData() {
    if (!this.id) return;
    this.isLoading = true;
    this.error = null;
    
    forkJoin({
      request: this.sponsorshipService.getRequestById(this.id),
      history: this.sponsorshipService.getHistory(this.id).pipe(catchError(() => of([])))
    }).pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (res) => {
        this.request = res.request;
        this.history = res.history;
      },
      error: (err) => {
        console.error('Fetch Error:', err);
        this.error = 'Unable to load request details. The server might be down.';
      }
    });
  }

  canTakeAction(): boolean {
    if (!this.request || !this.userRole) return false;
    
    const status = this.request.status;
    if (this.userRole === 'SystemAdmin') return true;
    if (this.userRole === 'Manager' && status === 'PendingManagerApproval') return true;
    if (this.userRole === 'FinanceAdmin' && status === 'PendingFinanceReview') return true;
    if (this.userRole === 'Requestor' && (status === 'Draft' || status === 'NeedClarification')) return true;
    
    return false;
  }

  transition(newStatus: string) {
    if (!this.id) return;
    this.isLoading = true;
    
    this.sponsorshipService.transitionStatus(this.id, newStatus as RequestStatus, this.remarks).subscribe({
      next: () => {
        this.remarks = '';
        this.loadData(); // Refresh data
      },
      error: (err) => {
        console.error('Transition Error:', err);
        alert('Failed to update status: ' + (err.error?.message || 'Unknown error'));
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
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
      case 'NeedClarification': return 'bg-info text-dark';
      default: return 'bg-light text-dark border';
    }
  }
}