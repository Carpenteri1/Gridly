import { Component, Input, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Observable, of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { CardComponent } from '../card/card.component';
import { CardService } from '../../services/card_services/card.service';
import { GridService } from '../../services/grid_services/grid.service';
import { GridComponent } from './grid.component';

type GridComponentTestHarness = GridComponent & {
  Drop(event: unknown): void;
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
  let editMode: ReturnType<typeof signal<boolean>>;

  const cardServiceMock = {} as { cards$: Observable<CardModel[]> };
  const gridServiceMock = {} as { editMode: () => boolean };

  beforeEach(async () => {
    cards = [
      { id: 1, indexPosition: 1, name: 'One', url: 'https://one.example' },
      { id: 2, indexPosition: 2, name: 'Two', url: 'https://two.example' },
    ];
    cardServiceMock.cards$ = of(cards);
    editMode = signal(true);
    gridServiceMock.editMode = editMode.asReadonly();

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

  it('reorders items when drag-drop happens in edit mode', () => {
    const event = {
      container: { data: cards },
      previousIndex: 0,
      currentIndex: 1,
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event);

    expect(cards.map((card) => card.id)).toEqual([2, 1]);
    expect(cards.map((card) => card.indexPosition)).toEqual([1, 2]);
  });

  it('does not reorder items when edit mode is disabled', () => {
    editMode.set(false);

    const event = {
      container: { data: cards },
      previousIndex: 0,
      currentIndex: 1,
    } as never;

    (gridComponent as GridComponentTestHarness).Drop(event);

    expect(cards.map((card) => card.id)).toEqual([1, 2]);
  });
});
