import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container">
      <h1>Student Management</h1>
      <p>Student list coming soon...</p>
      <p>This will show all students from the API.</p>
    </div>
  `,
  styles: [`
    .page-container {
      padding: 40px;
      max-width: 1200px;
      margin: 0 auto;
    }
    h1 {
      color: #374151;
      margin-bottom: 16px;
    }
  `]
})
export class StudentListComponent {}
