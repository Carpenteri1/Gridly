import { Component } from '@angular/core';
import { BasicComponent } from '../Components/BasicComponent/basic.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [BasicComponent],
})
export class AppComponent {
  title = 'Gridly';
}
