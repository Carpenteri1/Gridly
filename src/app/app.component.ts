import { Component } from '@angular/core';
import { BasicComponent } from '../Components/basicComponent/basic.component';
import { HeaderComponent } from "../Components/headerComponent/header.component";

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [BasicComponent, HeaderComponent],
})
export class AppComponent {
  title = 'Gridly';
}
