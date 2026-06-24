import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject, firstValueFrom, Observable, ReplaySubject, take } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { EditCardModel } from '../../models/editCard.Model';
import { CardEndpointService } from '../endpoint_services/card.endpoint.service';

@Injectable({ providedIn: 'root' })
export class CardService {
  #api = inject(CardEndpointService);

  private readonly cardsSubject = new BehaviorSubject<CardModel[]>([]);

  readonly cardId = new ReplaySubject<number>(1);
  readonly cards$: Observable<CardModel[]>;

  readonly currentCards: Signal<CardModel[]>;

  constructor() {
    this.cards$ = this.cardsSubject.asObservable();
    this.currentCards = toSignal(this.cards$, { initialValue: [] as CardModel[] });
    this.refresh();
  }

  private batchEdit$ = (cards: CardModel[]) => this.#api.batchEdit(cards);
  private edit$ = (card: EditCardModel) => this.#api.edit(card);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  private add$ = (card: CardModel) => this.#api.add(card);

  batchEdit = (cards: CardModel[]) => firstValueFrom(this.batchEdit$(cards)).then(() => this.refresh());
  edit = (card: CardModel) => firstValueFrom(this.edit$({editCard: card, selectedDropDownIconValue: 2} as EditCardModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (card: CardModel) =>  firstValueFrom(this.add$(card)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());

  setRows(rows: CardModel[][], maxRowWidth = 0): void {
    this.cardsSubject.next(this.normalizeRows(rows, maxRowWidth).flatMap((row) => row));
  }

  toRows(cards: CardModel[], maxRowWidth = 0): CardModel[][] {
    const rows = new Map<number, CardModel[]>();
    cards.forEach((card) => {
      const rowIndex = Math.max((card.indexPosition ?? 1) - 1, 0);
      rows.set(rowIndex, [...(rows.get(rowIndex) ?? []), card]);
    });

    return this.normalizeRows(
      [...rows.entries()]
        .sort(([first], [second]) => first - second)
        .map(([, row]) => [...row].sort((a, b) => (a.rowPosition ?? 0) - (b.rowPosition ?? 0))),
      maxRowWidth,
    );
  }

  private normalizeRows(rows: CardModel[][], maxRowWidth: number): CardModel[][] {
    const normalizedRows = rows.map((row) => [...row]);

    if (maxRowWidth > 0) {
      for (let rowIndex = 0; rowIndex < normalizedRows.length; rowIndex++) {
        const row = normalizedRows[rowIndex];
        while (row.length > 1 && this.getRowWidth(row) > maxRowWidth) {
          const overflowCard = row.pop();
          if (!overflowCard) break;
          normalizedRows[rowIndex + 1] = normalizedRows[rowIndex + 1] ?? [];
          normalizedRows[rowIndex + 1].unshift(overflowCard);
        }
      }
    }

    return normalizedRows
      .filter((row) => row.length > 0)
      .map((row, rowIndex) =>
        row.map((card, rowPosition) => ({
          ...card,
          indexPosition: rowIndex + 1,
          rowPosition: rowPosition + 1,
        })),
      );
  }

  private getRowWidth(row: CardModel[]): number {
    const cardsWidth = row.reduce((width, card) =>
        width + card.settings!.width, 0,
    );

    return cardsWidth + Math.max(row.length - 1, 0);
  }

  refresh(): void {
    this.#api.get().pipe(take(1))
      .subscribe((cards) =>
        this.cardsSubject.next(cards));
  }
}
