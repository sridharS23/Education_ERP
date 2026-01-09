import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'students',
    loadComponent: () => import('./features/students/student-list/student-list.component').then(m => m.StudentListComponent)
  },
  {
    path: 'faculty',
    loadComponent: () => import('./features/faculty/faculty-list/faculty-list.component').then(m => m.FacultyListComponent)
  },
  {
    path: '**',
    redirectTo: '/login'
  }
];
