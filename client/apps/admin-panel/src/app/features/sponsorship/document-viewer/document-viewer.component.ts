import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-document-viewer',
    standalone: true,
    imports: [CommonModule, RouterModule],
    template: `
    <div class="min-vh-100 bg-dark d-flex flex-column">
      <div class="p-3 bg-secondary text-white d-flex justify-content-between align-items-center shadow">
        <h5 class="mb-0 text-truncate" style="max-width: 70%;">Document Preview: {{ fileName }}</h5>
        <div class="d-flex gap-2">
          <a *ngIf="fullUrl" [href]="fullUrl" target="_blank" class="btn btn-sm btn-info">
            <i class="bi bi-download me-1"></i> Download
          </a>
          <button class="btn btn-sm btn-light" (click)="goBack()">Close</button>
        </div>
      </div>
      <div class="flex-grow-1 d-flex justify-content-center align-items-center overflow-auto p-3">
        <ng-container *ngIf="fileName; else noFileTemplate">
          <ng-container *ngIf="isImage; else pdfTemplate">
            <img [src]="fullUrl" class="img-fluid rounded shadow-lg" style="max-height: 85vh;">
          </ng-container>
          <ng-template #pdfTemplate>
            <iframe *ngIf="safeUrl" [src]="safeUrl" width="100%" height="100%" class="border-0 bg-white rounded shadow"></iframe>
          </ng-template>
        </ng-container>
        <ng-template #noFileTemplate>
           <div class="text-white text-center">
              <i class="bi bi-file-earmark-x fs-1"></i>
              <p class="mt-2">No document specified for preview.</p>
           </div>
        </ng-template>
      </div>
    </div>
  `,
    styles: [`
    :host { display: block; height: 100vh; }
    iframe { min-height: 85vh; width: 100%; }
  `]
})
export class DocumentViewerComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private sanitizer = inject(DomSanitizer);

    fileName: string | null = null;
    fullUrl: string = '';
    safeUrl?: SafeResourceUrl;
    isImage = false;

    ngOnInit(): void {
        // Switch to queryParams to avoid dots in path segments causing issues with Vite
        this.route.queryParams.subscribe(params => {
            this.fileName = params['file'];
            if (this.fileName) {
                this.fullUrl = `${environment.apiUrl.replace('/api/v1', '')}/uploads/${this.fileName}`;
                this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.fullUrl);

                const ext = this.fileName.split('.').pop()?.toLowerCase();
                this.isImage = ['png', 'jpg', 'jpeg', 'gif', 'svg'].includes(ext || '');
            }
        });
    }

    goBack(): void {
        window.history.back();
    }
}
