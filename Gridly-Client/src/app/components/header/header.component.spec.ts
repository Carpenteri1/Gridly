import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
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

  const versionServiceMock = {
    version$: of({ id: 1, name: 'v1.0.0' }),
  };

  const gridServiceMock = {
    inEditMode: editMode.asReadonly(),
    toggleEdit: jest.fn(() => editMode.update((value) => !value)),
    setEditMode: jest.fn((value: boolean) => editMode.set(value)),
    addCardToFirstAvailableRow: jest.fn(),
    batchSave: jest.fn(),
    currentRowColumns: jest.fn(() => []),
  };

  beforeEach(async () => {
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
