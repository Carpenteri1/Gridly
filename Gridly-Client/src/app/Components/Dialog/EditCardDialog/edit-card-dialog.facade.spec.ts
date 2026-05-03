import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { IconService } from '../../../Services/Icon.service';
import { ComponentRulesService } from '../../../Services/component-rules.service';
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
    facade.componentData.name = '';
    facade.componentData.url = '';
    expect(facade.canSubmit).toBe(false);

    facade.componentData.name = 'Widget';
    facade.componentData.url = 'https://widget.example';
    expect(facade.canSubmit).toBe(true);
    expect(componentRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.componentData,
      name: 'Widget',
      url: 'https://widget.example',
    });
  });

  it('keeps whitespace-only values blocked when checking submit readiness', () => {
    facade.componentData.name = '   ';
    facade.componentData.url = '\t';

    expect(facade.canSubmit).toBe(false);
    expect(componentRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.componentData,
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

    expect(facade.componentData.iconData?.materialIcon).toBe('settings');
  });

  it('resets and builds the submit payload with the widget id', () => {
    facade.reset({ name: 'Alpha', url: 'https://alpha.example' });
    const payload = facade.buildSubmitPayload(42);

    expect(payload.id).toBe(42);
    expect(payload.name).toBe('Alpha');
    expect(payload.url).toBe('https://alpha.example');
  });
});
