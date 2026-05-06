import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { CardService } from '../../services/card_services/card.service';
import { GridService } from '../../services/grid_services/grid.service';
import { VersionService } from '../../services/version_services/version.service';
import { HeaderComponent } from './header.component';

type HeaderComponentTestHarness = HeaderComponent & {
  add(card: CardModel): Promise<void>;
  addDialogActive: boolean;
  reloadPage(): void;
  setEditMode(): void;
};

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;
  let headerComponent: HeaderComponent;

  const cardServiceMock = {
    add: jest.fn(),
    component$: of(undefined),
  };

  const versionServiceMock = {
    version$: of({ id: 1, name: 'v1.0.0' }),
  };

  const gridServiceMock = {
    toggle: jest.fn(),
  };

  beforeEach(async () => {
    cardServiceMock.add.mockResolvedValue(undefined);
    cardServiceMock.add.mockClear();
    gridServiceMock.toggle.mockClear();

    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        { provide: CardService, useValue: cardServiceMock },
        { provide: VersionService, useValue: versionServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    headerComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('toggles the menu signal through the public method', () => {
    expect(headerComponent.showMenu()).toBe(false);

    headerComponent.toggleMenu();
    expect(headerComponent.showMenu()).toBe(true);

    headerComponent.toggleMenu();
    expect(headerComponent.showMenu()).toBe(false);
  });

  it('delegates add and edit mode actions to the injected services', async () => {
    const card = new CardModel();

    (headerComponent as HeaderComponentTestHarness).addDialogActive = true;

    await (headerComponent as HeaderComponentTestHarness).add(card);
    (headerComponent as HeaderComponentTestHarness).setEditMode();

    expect(cardServiceMock.add).toHaveBeenCalledWith(card);
    expect((headerComponent as HeaderComponentTestHarness).addDialogActive).toBe(false);
    expect(gridServiceMock.toggle).toHaveBeenCalled();
  });

  it('renders the client title', () => {
    expect((fixture.nativeElement as HTMLElement).querySelector('h1')?.textContent).toContain('Gridly');
  });
});
