import { Component } from '@angular/core';
import { GridComponent } from './Components/Grid/grid.component';
import { HeaderComponent } from "./Components/Header/header.component";
import {TextStringsUtil} from "./Constants/text.strings.util";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  standalone: true,
  imports: [GridComponent, HeaderComponent]
})
export class AppComponent {
  title = TextStringsUtil.ClientTitle;
}
