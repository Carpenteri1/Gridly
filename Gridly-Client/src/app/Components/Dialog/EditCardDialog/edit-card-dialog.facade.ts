import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../dtos/searchIconsResultDto';
import { CardModel } from '../../../models/card.Model';
import { IconService } from '../../../services/Icon.service';
import { IconModel } from '../../../models/icon.Model';
import { ComponentRulesService } from '../../../services/component-rules.service';

@Injectable()
export class EditCardDialogFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  card: CardModel = new CardModel();

  #iconService = inject(IconService);
  #componentRulesService = inject(ComponentRulesService);

  constructor() {
    this.icons$ = this.#iconService.icons$;
  }

  get canSubmit(): boolean {
    return this.#componentRulesService.hasRequiredFields({
      ...this.card,
      name: this.card.name.trim(),
      url: this.card.url.trim(),
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

    this.card.iconData = iconData;
    this.card.iconData.materialIcon = event;
  }

  reset(initial?: Partial<CardModel>): void {
    this.card = Object.assign(new CardModel(), initial ?? {});
  }

  buildSubmitPayload(id: number): CardModel {
    this.card.id = id;
    return this.card;
  }
}
