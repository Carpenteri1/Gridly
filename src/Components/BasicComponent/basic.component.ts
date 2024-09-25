import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';  // Import CommonModule for *ngFor
import { SharedService } from '../../app/shared.service';

@Component({
  selector: 'basic-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './basic.component.html',
  styleUrls: ['./basic.component.css'],
})
export class BasicComponent {
  constructor(public sharedService: SharedService) {}
}