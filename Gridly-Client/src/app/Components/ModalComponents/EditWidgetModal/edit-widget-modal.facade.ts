import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../DTOs/SearchIconsResultDto';
import { ComponentModel } from '../../../Models/Component.Model';
import { IconService } from '../../../Services/Icon.service';
import { IconModel } from '../../../Models/Icon.Model';

@Injectable()
export class EditWidgetModalFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  componentData: ComponentModel = new ComponentModel();

  #iconService = inject(IconService);

  constructor() {
    this.icons$ = this.#iconService.icons$;
  }

  get canSubmit(): boolean {
    return this.componentData.name.trim().length > 0 && this.componentData.url.trim().length > 0;
  }

  onSearch(input: string): void {
    this.#iconService.search(input);
  }

  setIcon(event: string): void {

    var iconData = new IconModel();
    iconData.materialIcon = event;
    iconData.type = "";
    iconData.name = "";
    iconData.base64Data = "";
    iconData.type = "";

    this.componentData.iconData = iconData;
    this.componentData.iconData.materialIcon = event;
  }

  reset(initial?: Partial<ComponentModel>): void {
    //this.componentData = MapComponentData.Override(initial ?? {});
  }

  buildSubmitPayload(widgetId: number): ComponentModel {
    this.componentData.id = widgetId;
    return this.componentData;
  }
}
