import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../DTOs/SearchIconsResultDto';
import { ComponentModel } from '../../../Models/Component.Model';
import { IconService } from '../../../Services/Icon.service';
import { IconModel } from '../../../Models/Icon.Model';
import { ComponentRulesService } from '../../../Services/component-rules.service';

@Injectable()
export class EditWidgetModalFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  componentData: ComponentModel = new ComponentModel();

  #iconService = inject(IconService);
  #componentRulesService = inject(ComponentRulesService);

  constructor() {
    this.icons$ = this.#iconService.icons$;
  }

  get canSubmit(): boolean {
    return this.#componentRulesService.hasRequiredFields({
      ...this.componentData,
      name: this.componentData.name.trim(),
      url: this.componentData.url.trim(),
    });
  }

  onSearch(input: string): void {
    this.#iconService.search(input);
  }

  setIcon(event: string): void {

    const iconData = new IconModel();
    iconData.materialIcon = event;
    iconData.type = "";
    iconData.name = "";
    iconData.base64Data = "";
    iconData.type = "";

    this.componentData.iconData = iconData;
    this.componentData.iconData.materialIcon = event;
  }

  reset(initial?: Partial<ComponentModel>): void {
    this.componentData = Object.assign(new ComponentModel(), initial ?? {});
  }

  buildSubmitPayload(widgetId: number): ComponentModel {
    this.componentData.id = widgetId;
    return this.componentData;
  }
}
