import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { VersionService } from '../../services/version_services/version.service';
import { LocaleStringsService } from '../../services/locale_services/locale-strings.service';
import { HeaderComponent } from './header.component';

type HeaderComponentTestHarness = HeaderComponent & {
  add(card: CardModel): void;
  addDialogActive: boolean;
};

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;
  let headerComponent: HeaderComponent;
  const editMode = signal(false);

  const versionServiceMock = {
    version$: of({ id: 1, name: 'v1.0.0' }),
  };

  const gridServiceMock = {
    inEditMode: editMode.asReadonly(),
    toggleEdit: jest.fn(() => editMode.update((value) => !value)),
    setEditMode: jest.fn((value: boolean) => editMode.set(value)),
    addCardToFirstAvailableRow: jest.fn(),
    batchSave: jest.fn(() => Promise.resolve()),
    currentRowColumns: jest.fn(() => []),
  };

  const localeStringsServiceMock = {
    locale: {
      app: { text: { title: 'Gridly' } },
      menu: {
        text: {
          addCardButtonTitle: 'Add Card',
          saveButtonTitle: 'Save',
          editButtonTitle: 'Edit',
          exitEditButtonTitle: 'Exit Edit',
          dropDownDragTitle: 'Move cards',
          dropDownResizeTitle: 'Resize cards',
          addProviderKeysButtonTitle: 'Add provider key',
        },
      },
      addCardPicker: { text: { title: 'Add Card', description: 'Pick card type for your dashboard' } },
      widget: { url: { get: '/api/widget/get' } },
      providerKey: {
        url: {
          getLocalStatus: '/api/providerkeys/local/provider/status',
          getRemoteStatus: '/api/providerkeys/remote/provider/status',
          save: '/api/providerkeys/save',
        },
      },
      apiKeyDialog: {
        text: {
          title: 'Add weather provider key',
          description: 'Paste your weather provider API key. It is stored securely and never shown again.',
          foundApiKeyAt: 'You can find your key at https://www.visualcrossing.com',
          invalidMessage: 'The stored key was rejected. Please paste a new one.',
          inputLabel: 'API key',
          saveBtnTitle: 'Save',
          cancelBtnTitle: 'Cancel',
          saveFailedMessage: 'Failed to save the API key. Please try again.',
        },
      },
    },
  };

  beforeEach(async () => {
    editMode.set(false);
    gridServiceMock.toggleEdit.mockClear();
    gridServiceMock.setEditMode.mockClear();
    gridServiceMock.addCardToFirstAvailableRow.mockClear();
    gridServiceMock.batchSave.mockClear();

    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        { provide: VersionService, useValue: versionServiceMock },
        { provide: GridService, useValue: gridServiceMock },
        { provide: LocaleStringsService, useValue: localeStringsServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    headerComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('toggles edit mode through the public method', () => {
    expect(headerComponent.editActive()).toBe(false);

    headerComponent.toggleMenu();

    expect(gridServiceMock.toggleEdit).toHaveBeenCalledTimes(1);
    expect(headerComponent.editActive()).toBe(true);

    headerComponent.toggleMenu();

    expect(gridServiceMock.toggleEdit).toHaveBeenCalledTimes(2);
    expect(headerComponent.editActive()).toBe(false); 
  });

  it('waits for the save to complete before exiting edit mode and reloading', async () => {
    await headerComponent.save();

    expect(gridServiceMock.batchSave).toHaveBeenCalledTimes(1);
    expect(gridServiceMock.toggleEdit).toHaveBeenCalledTimes(1);

    const batchSaveOrder = gridServiceMock.batchSave.mock.invocationCallOrder[0];
    const toggleEditOrder = gridServiceMock.toggleEdit.mock.invocationCallOrder[0];
    expect(batchSaveOrder).toBeLessThan(toggleEditOrder);
  });

  it('adds cards to the current grid rows and closes the dialog', () => {
    const card = new CardModel();

    (headerComponent as HeaderComponentTestHarness).addDialogActive = true;

    (headerComponent as HeaderComponentTestHarness).add(card);

    expect(gridServiceMock.addCardToFirstAvailableRow).toHaveBeenCalledWith(card);
    expect((headerComponent as HeaderComponentTestHarness).addDialogActive).toBe(false);
  });

  it('Inject service and set edit mode', () => {
    expect(headerComponent.editActive()).toBe(false);

    gridServiceMock.setEditMode(true);
    expect(gridServiceMock.inEditMode()).toBe(true);
  
    gridServiceMock.setEditMode(false);
    expect(gridServiceMock.inEditMode()).toBe(false);
  });

  it('renders the client title', () => {
    expect((fixture.nativeElement as HTMLElement).querySelector('h1')?.textContent).toContain('Gridly');
  });
});
