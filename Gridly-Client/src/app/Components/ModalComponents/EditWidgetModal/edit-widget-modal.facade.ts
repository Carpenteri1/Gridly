import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../DTOs/SearchIconsResultDto';
import { ComponentModel } from '../../../Models/Component.Model';
import { IconService } from '../../../Services/Icon.service';
import { ModalType } from '../../../Types/modaltypes.enum';
import { MapComponentData } from '../../../Utils/componentModel.factory';
import { MapIconData } from '../../../Utils/iconModel.factory';
import { IconModel } from '../../../Models/Icon.Model';

@Injectable()
export class EditWidgetModalFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  componentData: ComponentModel = MapComponentData();

  #iconService = inject(IconService);

  constructor() {
    this.icons$ = this.#iconService.icons$;
  }

  get canSubmit(): boolean {
    return this.componentData.name.trim().length > 0 && this.componentData.url.trim().length > 0;
  }

  onSearch(input: string): void {
    this.#iconService.searchInput$.next(input);
  }

  setIcon(event: string): void {
    this.componentData.iconData = MapIconData();
    this.componentData.iconData.materialIcon = event;
  }

  reset(initial?: Partial<ComponentModel>): void {
    this.componentData = MapComponentData.Override(initial ?? {});
  }

  buildSubmitPayload(widgetId: number): { component: ComponentModel; modalType: ModalType } {
    this.componentData.id = widgetId;
    return {
      component: MapComponentData(this.componentData),
      modalType: ModalType.Edit,
    };
  }
}
