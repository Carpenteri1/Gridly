import { Component } from '@angular/core';
import { GridComponent } from './components/grid/grid.component';
import { HeaderComponent } from "./components/header/header.component";
import {TextStringsUtil} from "./constants/text.strings.util";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  standalone: true,
  imports: [GridComponent, HeaderComponent]
})
export class AppComponent {
  title = TextStringsUtil.ClientTitle;
  isEditMode = false;
}
