import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipType } from '@client/core-shared';

@Component({
  selector: 'app-request-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './request-form.component.html'
})
export class RequestFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private sponsorshipService = inject(SponsorshipService);
  private router = inject(Router);

  requestForm!: FormGroup;
  sponsorshipTypes = Object.values(SponsorshipType);
  selectedFile: File | null = null;
  isSubmitting = false;

  ngOnInit(): void {
    this.requestForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      department: ['', [Validators.required]],
      type: [SponsorshipType.Event, [Validators.required]],
      eventName: ['', [Validators.required]],
      eventDate: ['', [Validators.required]],
      requestedAmount: [0, [Validators.required, Validators.min(1)]],
      purpose: ['', [Validators.required]],
      expectedBusinessBenefit: ['']
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  onSubmit(): void {
    if (this.requestForm.valid) {
      this.createRequest('PendingManagerApproval');
    }
  }

  saveDraft(): void {
    if (this.requestForm.valid) {
      this.createRequest('Draft');
    }
  }

  private createRequest(status: string): void {
    if (this.isSubmitting) return;
    this.isSubmitting = true;
    
    const data = { ...this.requestForm.value, status };
    this.sponsorshipService.createRequest(data, this.selectedFile || undefined).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/sponsorship']);
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Error creating request', err);
      }
    });
  }
}
