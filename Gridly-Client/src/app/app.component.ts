import { Component, inject } from '@angular/core';
import { GridComponent } from './components/grid/grid.component';
import { HeaderComponent } from "./components/header/header.component";
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [GridComponent, HeaderComponent]
})
export class AppComponent {
  title = inject(TranslateService).instant('app.text.title');
  isEditMode = false;
}
