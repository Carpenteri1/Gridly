import { Component, Input, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject, Observable } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { CardComponent } from '../card/card.component';
import { CardService } from '../../services/card_services/card.service';
import { GridService } from '../../services/grid_services/grid.service';
import { GridComponent } from './grid.component';

type GridComponentTestHarness = GridComponent & {
  Drop(event: unknown, rows: CardModel[][], rowIndex: number): void;
};

@Component({
  selector: 'app-card-component',
  template: '',
  standalone: true,
})
class MockCardComponent {
  @Input({ required: true }) card!: CardModel;
}

describe('GridComponent', () => {
  let fixture: ComponentFixture<GridComponent>;
  let gridComponent: GridComponent;
  let cards: CardModel[];
  let cardsSubject: BehaviorSubject<CardModel[]>;
  let editMode: ReturnType<typeof signal<boolean>>;

  const cardServiceMock = {} as {
    cards$: Observable<CardModel[]>;
    setRows: jest.Mock;
    toRows: jest.Mock;
  };
  const gridServiceMock = {} as { inEditMode: () => boolean };

  beforeEach(async () => {
    cards = [
      { id: 1, indexPosition: 1, name: 'One', url: 'https://one.example' },
      { id: 2, indexPosition: 2, name: 'Two', url: 'https://two.example' },
    ];
    cardsSubject = new BehaviorSubject<CardModel[]>(cards);
    cardServiceMock.cards$ = cardsSubject.asObservable();
    cardServiceMock.setRows = jest.fn();
    cardServiceMock.toRows = jest.fn((cardsToGroup: CardModel[]) => [cardsToGroup]);
    editMode = signal(true);
    gridServiceMock.inEditMode = editMode.asReadonly();

    TestBed.overrideComponent(GridComponent, {
      remove: { imports: [CardComponent] },
      add: { imports: [MockCardComponent] },
    });

    await TestBed.configureTestingModule({
      imports: [GridComponent],
      providers: [
        { provide: CardService, useValue: cardServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(GridComponent);
    gridComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders one card component per returned component', () => {
    const items = fixture.nativeElement.querySelectorAll('app-card-component');

    expect(items).toHaveLength(2);
  });

  it('reorders card when drag-drop happens in edit mode', () => {
    const event = {
      previousContainer: { id: 'card-row-0' },
      previousIndex: 0,
      currentIndex: 1,
      item: { data: cards[0] },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, [cards], 0);

    expect(cardServiceMock.setRows).toHaveBeenCalledWith([[cards[1], cards[0]]], 0);
  });

  it('does not reorder card when edit mode is disabled', () => {
    editMode.set(false);

    const event = {
      previousContainer: { id: 'card-row-0' },
      previousIndex: 0,
      currentIndex: 1,
      item: { data: cards[0] },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, [cards], 0);

    expect(cardServiceMock.setRows).not.toHaveBeenCalled();
  });

  it('moves a card into a new row', () => {
    const event = {
      previousContainer: { id: 'card-row-0' },
      previousIndex: 1,
      currentIndex: 0,
      item: { data: cards[1] },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, [cards], 1);

    expect(cardServiceMock.setRows).toHaveBeenCalledWith([[cards[0]], [cards[1]]], 0);
  });

  it('keeps the card DOM element stable when a resized card object is emitted', () => {
    const firstCardElement = fixture.nativeElement.querySelector('.grid-card-style') as HTMLElement;

    cardsSubject.next([
      {
        ...cards[0],
        settings: { width: 500, height: 300, imageHidden: false, titleHidden: false },
      },
      cards[1],
    ]);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.grid-card-style')).toBe(firstCardElement);
  });
});
