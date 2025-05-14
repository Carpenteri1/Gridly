import { Component } from '@angular/core';
import { BasicComponent } from './Components/BasicComponent/basic.component';
import { HeaderComponent } from "./Components/Header/header.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  standalone: true,
  imports: [BasicComponent, HeaderComponent]
})
export class AppComponent {
  title = 'Gridly';
}
