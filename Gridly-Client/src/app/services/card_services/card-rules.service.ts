import { Injectable } from '@angular/core';
import { RegexStringsUtil } from '../../constants/regex.strings.util';
import { CardModel } from '../../models/card.Model';

@Injectable({ providedIn: 'root' })
export class CardRulesService {
  private getSettings(card: CardModel | null | undefined) {
    return card?.settings ?? card?.settings;
  }

  hasRequiredFields(card: CardModel | null | undefined): boolean {
    return card !== undefined &&
      card !== null &&
      card.name !== '' &&
      card.url !== '';
  }

  hasValidName(name: string | null | undefined): boolean {
    return name !== undefined &&
      name !== null &&
      name !== '' &&
      RegexStringsUtil.namePattern.test(name);
  }

  hasValidUrl(url: string | null | undefined): boolean {
    return url !== undefined &&
      url !== null &&
      url !== '' &&
      RegexStringsUtil.urlPattern.test(url);
  }

  hasValidCardData(card: CardModel | null | undefined): boolean {
    if (card === undefined ||
      card === null ||
      card.name === '' ||
      card.url === '')
      return false;

    return this.hasValidName(card.name) &&
      this.hasValidUrl(card.url);
  }

  hasIconData(card: CardModel | null | undefined): boolean {
    return card !== undefined &&
      card !== null &&
      card.iconData !== undefined &&
      card.iconData.name !== '' &&
      card.iconData.type !== undefined &&
      card.iconData.base64Data !== '' &&
      !this.getSettings(card)?.imageHidden;
  }

  hasIconUrl(card: CardModel | null | undefined): boolean {
    return card !== undefined &&
      card !== null &&
      card.iconUrl !== undefined &&
      card.iconUrl !== '' &&
      RegexStringsUtil.iconUrlPattern.test(card.iconUrl) &&
      !this.getSettings(card)?.imageHidden;
  }

  hasMaterialIcon(card: CardModel | null | undefined): boolean {
    return card !== undefined &&
      card !== null &&
      card.iconData?.materialIcon !== undefined &&
      card.iconData.materialIcon !== '' &&
      !this.getSettings(card)?.imageHidden;
  }
}
