import { TestBed } from '@angular/core/testing';
import { CardModel } from '../../models/card.Model';
import { CardRulesService } from './card-rules.service';

describe('CardRulesService', () => {
  let service: CardRulesService;

  const componentWithIconData: CardModel = {
    id: 1,
    indexPosition: 1,
    name: 'Alpha',
    url: 'https://alpha.example',
    iconData: { name: 'dashboard', type: 'svg', base64Data: 'abc', materialIcon: 'dashboard' },
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };
  const componentWithIconUrl: CardModel = {
    id: 2,
    indexPosition: 2,
    name: 'Beta',
    url: 'https://beta.example',
    iconUrl: 'https://cdn.example/icon.png',
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CardRulesService],
    });

    service = TestBed.inject(CardRulesService);
  });

  it('checks required fields and shared name/url validation rules', () => {
    expect(service.hasRequiredFields(componentWithIconData)).toBe(true);
    expect(service.hasValidName(componentWithIconData.name)).toBe(true);
    expect(service.hasValidUrl(componentWithIconData.url)).toBe(true);
    expect(service.hasValidCardData(componentWithIconData)).toBe(true);

    expect(service.hasRequiredFields({ ...componentWithIconData, name: '' })).toBe(false);
    expect(service.hasValidName('Alpha-1')).toBe(false);
    expect(service.hasValidUrl('not-a-url')).toBe(false);
    expect(service.hasValidCardData({ ...componentWithIconData, url: 'not-a-url' })).toBe(false);
  });

  it('recognizes icon data, icon urls, and material icons when visible', () => {
    expect(service.hasIconData(componentWithIconData)).toBe(true);
    expect(service.hasMaterialIcon(componentWithIconData)).toBe(true);
    expect(service.hasIconUrl(componentWithIconUrl)).toBe(true);
  });

  it('rejects icon sources when they are hidden or invalid', () => {
    expect(service.hasIconData({ ...componentWithIconData, settings: { ...componentWithIconData.settings!, imageHidden: true } })).toBe(false);
    expect(service.hasMaterialIcon({ ...componentWithIconData, iconData: { ...componentWithIconData.iconData!, materialIcon: '' } })).toBe(false);
    expect(service.hasIconUrl({ ...componentWithIconUrl, iconUrl: 'icon.png' })).toBe(false);
  });

  it('tolerates nullish and partially initialized component data', () => {
    expect(service.hasRequiredFields(undefined)).toBe(false);
    expect(service.hasValidCardData(null)).toBe(false);
    expect(service.hasIconData({ ...componentWithIconData, iconData: undefined })).toBe(false);
    expect(service.hasIconUrl({ ...componentWithIconUrl, settings: undefined })).toBe(true);
    expect(service.hasMaterialIcon({ ...componentWithIconData, settings : undefined })).toBe(true);
  });
});
