import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../models/card.Model';
import { VersionModel } from '../../models/version.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { VersionService } from '../../services/version_services/version.service';
import { HeaderComponent } from './header.component';

type HeaderComponentTestHarness = HeaderComponent & {
  add(card: CardModel): void;
  addDialogActive: boolean;
};

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;
  let headerComponent: HeaderComponent;
  const editMode = signal(false);
  const currentVersion = signal<VersionModel | undefined>(undefined);

  const versionServiceMock = {
    currentVersion: currentVersion.asReadonly(),
  };

  const gridServiceMock = {
    inEditMode: editMode.asReadonly(),
    toggleEdit: jest.fn(() => editMode.update((value) => !value)),
    setEditMode: jest.fn((value: boolean) => editMode.set(value)),
    addCardToFirstAvailableRow: jest.fn(),
    batchSave: jest.fn(() => Promise.resolve()),
    currentRowColumns: jest.fn(() => []),
  };

  beforeEach(async () => {
    editMode.set(false);
    currentVersion.set(undefined);
    gridServiceMock.toggleEdit.mockClear();
    gridServiceMock.setEditMode.mockClear();
    gridServiceMock.addCardToFirstAvailableRow.mockClear();
    gridServiceMock.batchSave.mockClear();

    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        { provide: VersionService, useValue: versionServiceMock },
        { provide: GridService, useValue: gridServiceMock },
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

  it('shows the new version banner when a new release is available', () => {
    currentVersion.set({ tag_name: 'v2.0.0', newRelease: true } as VersionModel);
    fixture.detectChanges();

    const banner = (fixture.nativeElement as HTMLElement).querySelector('.badge');
    expect(banner?.textContent).toContain('v2.0.0');
  });

  it('hides the new version banner when there is no new release', () => {
    currentVersion.set({ tag_name: 'v1.0.0', newRelease: false } as VersionModel);
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).querySelector('.badge')).toBeNull();
  });
});
