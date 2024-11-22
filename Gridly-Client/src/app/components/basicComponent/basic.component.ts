import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';  // Import CommonModule for *ngFor
import { SharedService } from '../../shared.service';

@Component({
  selector: 'basic-component',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './basic.component.html',
  styleUrls: ['./basic.component.css'],
})
export class BasicComponent {
  constructor(public sharedService: SharedService) {}
  Remove(id: number): void {
    this.sharedService.Remove(id);
  }
}