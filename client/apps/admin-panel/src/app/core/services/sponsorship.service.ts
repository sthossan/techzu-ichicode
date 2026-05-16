import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SponsorshipRequest, RequestStatus, WorkflowHistory } from '@client/core-shared';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SponsorshipService {
  private apiUrl = `${environment.apiUrl}/sponsorship`;

  constructor(private http: HttpClient) {}

  createRequest(data: any, file?: File): Observable<any> {
    const formData = new FormData();
    // Append all data fields
    Object.keys(data).forEach(key => {
      if (data[key] !== null && data[key] !== undefined) {
        formData.append(key, data[key]);
      }
    });
    // Append file if exists
    if (file) {
      formData.append('file', file);
    }
    return this.http.post(`${this.apiUrl}`, formData);
  }

  getMyRequests(): Observable<SponsorshipRequest[]> {
    return this.http.get<any>(`${this.apiUrl}/my-requests`).pipe(
      map(res => res.data)
    );
  }

  getPendingApprovals(): Observable<SponsorshipRequest[]> {
    return this.http.get<any>(`${this.apiUrl}/pending-approvals`).pipe(
      map(res => res.data)
    );
  }

  getRequestById(id: string): Observable<SponsorshipRequest> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(res => res.data)
    );
  }

  transitionStatus(id: string, newStatus: RequestStatus, remarks?: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/transition`, { newStatus, remarks });
  }

  getHistory(id: string): Observable<WorkflowHistory[]> {
    return this.http.get<any>(`${this.apiUrl}/${id}/history`).pipe(
      map(res => res.data)
    );
  }

  uploadDocument(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${environment.apiUrl}/files/upload`, formData);
  }
}
