import { Injectable } from '@angular/core';
import { RegexStringsUtil } from '../Constants/regex.strings.util';
import { CardModel } from '../Models/Card.Model';

@Injectable({ providedIn: 'root' })
export class ComponentRulesService {
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

  hasValidComponentData(component: CardModel | null | undefined): boolean {
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
      !component.componentSettings?.imageHidden;
  }

  hasIconUrl(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.iconUrl !== undefined &&
      component.iconUrl !== '' &&
      RegexStringsUtil.iconUrlPattern.test(component.iconUrl) &&
      !component.componentSettings?.imageHidden;
  }

  hasMaterialIcon(component: CardModel | null | undefined): boolean {
    return component !== undefined &&
      component !== null &&
      component.iconData?.materialIcon !== undefined &&
      component.iconData.materialIcon !== '' &&
      !component.componentSettings?.imageHidden;
  }
}
