import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { RequestListComponent } from './features/sponsorship/request-list/request-list.component';
import { RequestFormComponent } from './features/sponsorship/request-form/request-form.component';
import { RequestDetailComponent } from './features/sponsorship/request-detail/request-detail.component';
import { LoginComponent } from './features/auth/login/login.component';
import { DocumentViewerComponent } from './features/sponsorship/document-viewer/document-viewer.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'preview', component: DocumentViewerComponent },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'sponsorship',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        component: RequestListComponent
      },
      {
        path: 'new',
        component: RequestFormComponent
      },
      {
        path: ':id',
        component: RequestDetailComponent
      }
    ]
  }
];
