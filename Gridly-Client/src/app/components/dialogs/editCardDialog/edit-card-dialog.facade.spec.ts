import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { IconService } from '../../../services/Icon.service';
import { ComponentRulesService } from '../../../services/card-rules.service';
import { EditCardDialogFacade } from './edit-card-dialog.facade';

describe('EditCardDialogFacade', () => {
  let facade: EditCardDialogFacade;

  const iconServiceMock = {
    icons$: of({ icons: ['home'] }),
    search: jest.fn(),
  };

  const componentRulesServiceMock = {
    hasRequiredFields: jest.fn((component: { name?: string; url?: string }) =>
      Boolean(component.name?.trim().length) && Boolean(component.url?.trim().length)
    ),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    TestBed.configureTestingModule({
      providers: [
        EditCardDialogFacade,
        { provide: IconService, useValue: iconServiceMock },
        { provide: ComponentRulesService, useValue: componentRulesServiceMock },
      ],
    });

    facade = TestBed.inject(EditCardDialogFacade);
  });

  it('validates submit readiness from the current form data', () => {
    facade.card.name = '';
    facade.card.url = '';
    expect(facade.canSubmit).toBe(false);

    facade.card.name = 'Card';
    facade.card.url = 'https://card.example';
    expect(facade.canSubmit).toBe(true);
    expect(componentRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.card,
      name: 'Card',
      url: 'https://card.example',
    });
  });

  it('keeps whitespace-only values blocked when checking submit readiness', () => {
    facade.card.name = '   ';
    facade.card.url = '\t';

    expect(facade.canSubmit).toBe(false);
    expect(componentRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.card,
      name: '',
      url: '',
    });
  });

  it('forwards icon searches to the icon service', () => {
    facade.onSearch('mail');

    expect(iconServiceMock.search).toHaveBeenCalledWith('mail');
  });

  it('stores a selected material icon on the component payload', () => {
    facade.setIcon('settings');

    expect(facade.card.iconData?.materialIcon).toBe('settings');
  });

  it('resets and builds the submit payload with the card id', () => {
    facade.reset({ name: 'Alpha', url: 'https://alpha.example' });
    const payload = facade.buildSubmitPayload(42);

    expect(payload.id).toBe(42);
    expect(payload.name).toBe('Alpha');
    expect(payload.url).toBe('https://alpha.example');
  });
});
