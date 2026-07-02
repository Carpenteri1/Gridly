import { Component, Input, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { CardComponent } from '../card/card.component';
import { RowColumnModel } from '../../models/rowColumn.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { GridComponent } from './grid.component';

type GridComponentTestHarness = GridComponent & {
  Drop(event: unknown, rows: RowColumnModel[], rowIndex: number): void;
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
  let rows: RowColumnModel[];
  let rowsSubject: BehaviorSubject<RowColumnModel[]>;
  let editMode: ReturnType<typeof signal<boolean>>;

  const gridServiceMock = {} as {
    rows$: BehaviorSubject<RowColumnModel[]>;
    inEditMode: () => boolean;
    setRowsForView: jest.Mock;
  };

  beforeEach(async () => {
    cards = [
      { id: 1, indexPosition: 0, rowPosition: 1, name: 'One', url: 'https://one.example' },
      { id: 2, indexPosition: 1, rowPosition: 1, name: 'Two', url: 'https://two.example' },
    ];
    rows = [
      { id: 1, rowPosition: 1, cards },
    ];
    rowsSubject = new BehaviorSubject<RowColumnModel[]>(rows);
    editMode = signal(true);
    gridServiceMock.rows$ = rowsSubject;
    gridServiceMock.inEditMode = editMode.asReadonly();
    gridServiceMock.setRowsForView = jest.fn();

    TestBed.overrideComponent(GridComponent, {
      remove: { imports: [CardComponent] },
      add: { imports: [MockCardComponent] },
    });

    await TestBed.configureTestingModule({
      imports: [GridComponent],
      providers: [
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(GridComponent);
    gridComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('does not reorder card when edit mode is disabled', () => {
    editMode.set(false);

    const event = {
      previousContainer: { id: 'card-row-0' },
      previousIndex: 0,
      currentIndex: 1,
      item: { data: cards[0] },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, rows, 1);

    expect(gridServiceMock.setRowsForView).not.toHaveBeenCalled();
  });

  it('removes the source row and renumbers rows when its only card moves into an existing row', () => {
    const movedCard = { id: 1, indexPosition: 2, rowPosition: 1, name: 'One', url: 'https://one.example' };
    const secondCard = { id: 2, indexPosition: 1, rowPosition: 2, name: 'Two', url: 'https://two.example' };
    const thirdCard = { id: 3, indexPosition: 3, rowPosition: 2, name: 'Three', url: 'https://three.example' };
    const fourthCard = { id: 4, indexPosition: 0, rowPosition: 3, name: 'Four', url: 'https://four.example' };
    const rowColumns: RowColumnModel[] = [
      { id: 1, rowPosition: 1, cards: [movedCard] },
      { id: 2, rowPosition: 2, cards: [secondCard, thirdCard] },
      { id: 3, rowPosition: 3, cards: [fourthCard] },
    ];
    const event = {
      currentIndex: 1,
      item: { data: movedCard },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, rowColumns, 2);

    expect(gridServiceMock.setRowsForView).toHaveBeenCalledWith([
      {
        id: 2,
        rowPosition: 1,
        cards: [
          { ...secondCard, indexPosition: 0, rowPosition: 1 },
          { ...movedCard, indexPosition: 1, rowPosition: 1 },
          { ...thirdCard, indexPosition: 2, rowPosition: 1 },
        ],
      },
      {
        id: 3,
        rowPosition: 2,
        cards: [
          { ...fourthCard, indexPosition: 0, rowPosition: 2 },
        ],
      },
    ]);
  });

  it('removes the source row and renumbers rows when its only card moves into a new row', () => {
    const movedCard = { id: 1, indexPosition: 1, rowPosition: 1, name: 'One', url: 'https://one.example' };
    const secondCard = { id: 2, indexPosition: 1, rowPosition: 2, name: 'Two', url: 'https://two.example' };
    const rowColumns: RowColumnModel[] = [
      { id: 1, rowPosition: 1, cards: [movedCard] },
      { id: 2, rowPosition: 2, cards: [secondCard] },
    ];
    const event = {
      currentIndex: 0,
      item: { data: movedCard },
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event, rowColumns, 3);

    expect(gridServiceMock.setRowsForView).toHaveBeenCalledWith([
      {
        id: 2,
        rowPosition: 1,
        cards: [
          { ...secondCard, indexPosition: 0, rowPosition: 1 },
        ],
      },
      {
        id: 0,
        rowPosition: 2,
        cards: [
          { ...movedCard, indexPosition: 0, rowPosition: 2 },
        ],
      },
    ]);
  });

  it('keeps the card DOM element stable when a resized card object is emitted', () => {
    const firstCardElement = fixture.nativeElement.querySelector('.grid-card-style') as HTMLElement;

    rowsSubject.next([
      {
        ...rows[0],
        cards: [
          {
            ...cards[0],
            settings: { width: 500, height: 300, imageHidden: false, titleHidden: false },
          },
          cards[1],
        ],
      },
    ]);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.grid-card-style')).toBe(firstCardElement);
  });
});
