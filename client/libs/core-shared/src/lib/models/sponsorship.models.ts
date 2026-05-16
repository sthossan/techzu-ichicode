export enum UserRole {
  Requestor = 'Requestor',
  Manager = 'Manager',
  FinanceAdmin = 'FinanceAdmin',
  SystemAdmin = 'SystemAdmin'
}

export enum RequestStatus {
  Draft = 'Draft',
  PendingManagerApproval = 'PendingManagerApproval',
  PendingFinanceReview = 'PendingFinanceReview',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled',
  NeedClarification = 'NeedClarification'
}

export enum SponsorshipType {
  Event = 'Event',
  Conference = 'Conference',
  Research = 'Research',
  Community = 'Community',
  Other = 'Other'
}

export interface SponsorshipRequest {
  id: string;
  title: string;
  requestorId: string;
  requestorName?: string;
  department: string;
  type: SponsorshipType;
  eventName: string;
  eventDate: string;
  requestedAmount: number;
  purpose: string;
  documentUrl?: string;
  expectedBusinessBenefit?: string;
  remarks?: string;
  status: RequestStatus;
  createdAt: string;
}

export interface WorkflowHistory {
  id: string;
  requestId: string;
  fromStatus: RequestStatus;
  toStatus: RequestStatus;
  actionByName: string;
  actionDate: string;
  remarks?: string;
}
