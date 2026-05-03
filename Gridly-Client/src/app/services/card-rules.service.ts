import { Injectable } from '@angular/core';
import { RegexStringsUtil } from '../constants/regex.strings.util';
import { CardModel } from '../models/card.Model';

@Injectable({ providedIn: 'root' })
export class ComponentRulesService {
  private getSettings(component: CardModel | null | undefined) {
    return component?.settings ?? component?.componentSettings;
  }

  hasRequiredFields(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.name !== '' &&
      component.url !== '';
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

  hasValidCardData(component: CardModel | null | undefined): boolean {
    if (component === undefined ||
      component === null ||
      component.name === '' ||
      component.url === '')
      return false;

    return this.hasValidName(component.name) &&
      this.hasValidUrl(component.url);
  }

  hasIconData(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.iconData !== undefined &&
      component.iconData.name !== '' &&
      component.iconData.type !== undefined &&
      component.iconData.base64Data !== '' &&
      !this.getSettings(component)?.imageHidden;
  }

  hasIconUrl(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.iconUrl !== undefined &&
      component.iconUrl !== '' &&
      RegexStringsUtil.iconUrlPattern.test(component.iconUrl) &&
      !this.getSettings(component)?.imageHidden;
  }

  hasMaterialIcon(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.iconData?.materialIcon !== undefined &&
      component.iconData.materialIcon !== '' &&
      !this.getSettings(component)?.imageHidden;
  }
}
