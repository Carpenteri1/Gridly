import { Component, inject } from '@angular/core';
import { GridComponent } from './components/grid/grid.component';
import { HeaderComponent } from "./components/header/header.component";
import { LocaleStringsService } from "./services/locale_services/locale-strings.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [GridComponent, HeaderComponent]
})
export class AppComponent {
  title = inject(LocaleStringsService).locale.app.text.title;
  isEditMode = false;
}
