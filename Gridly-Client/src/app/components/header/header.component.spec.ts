import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { VersionService } from '../../services/version_services/version.service';
import { AddCardDialogComponent } from '../dialogs/addCardDialog/add-card-dialog.component';
import { ProviderKeyDialogComponent } from '../dialogs/apiKeyDialog/provider-key-dialog.component';
import { StubTranslatePipe } from '../../testing/stub-translate.pipe';
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

  const translateServiceMock = {
    instant: (key: string) => key,
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
        { provide: TranslateService, useValue: translateServiceMock },
      ],
    })
      .overrideComponent(HeaderComponent, { remove: { imports: [TranslatePipe] }, add: { imports: [StubTranslatePipe] } })
      .overrideComponent(AddCardDialogComponent, { remove: { imports: [TranslatePipe] }, add: { imports: [StubTranslatePipe] } })
      .overrideComponent(ProviderKeyDialogComponent, { remove: { imports: [TranslatePipe] }, add: { imports: [StubTranslatePipe] } })
      .compileComponents();

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
    expect((fixture.nativeElement as HTMLElement).querySelector('h1')?.textContent).toContain('app.text.title');
  });

  it('opens the add-provider-key dialog', () => {
    headerComponent.openAddProviderKeyDialog();

    expect(headerComponent.isAddProviderDialogOpen()).toBe(true);
  });

  it('closes the add-provider-key dialog when the matching dialog id is emitted', () => {
    headerComponent.openAddProviderKeyDialog();

    (headerComponent as unknown as { handleDialogChange(id: number): void }).handleDialogChange(0);

    expect(headerComponent.isAddProviderDialogOpen()).toBe(false);
  });

  it('leaves the add-provider-key dialog open when a non-matching dialog id is emitted', () => {
    headerComponent.openAddProviderKeyDialog();

    (headerComponent as unknown as { handleDialogChange(id: number): void }).handleDialogChange(1);

    expect(headerComponent.isAddProviderDialogOpen()).toBe(true);
  });
});
