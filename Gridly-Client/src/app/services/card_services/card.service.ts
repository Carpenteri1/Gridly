import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject, Observable, ReplaySubject, take } from 'rxjs';
import { CardModel } from '../../models/card.Model';
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

  setRows(rows: CardModel[][], maxRowWidth = 0): void {
    const normalizedRows = this.normalizeRows(rows, maxRowWidth);
    this.cardsSubject.next(normalizedRows.flat());
  }

  toRows(cards: CardModel[], maxRowWidth = 0): CardModel[][] {
    const rows = this.groupCardsByRow(cards);
    const sortedRows = this.getSortedRows(rows);

    return this.normalizeRows(sortedRows, maxRowWidth);
  }

  private groupCardsByRow(cards: CardModel[]): Map<number, CardModel[]> {
    const rows = new Map<number, CardModel[]>();

    for (const card of cards) {
      const rowIndex = Math.max((card.rowPosition ?? 1) - 1, 0);
      const row = rows.get(rowIndex) ?? [];
      row.push(card);
      rows.set(rowIndex, row);
    }

    return rows;
  }

  private getSortedRows(rows: Map<number, CardModel[]>): CardModel[][] {
    return [...rows.entries()]
      .sort(([first], [second]) => first - second)
      .map(([, row]) => [...row].sort((a, b) => (a.indexPosition ?? 0) - (b.indexPosition ?? 0)));
  }

  private normalizeRows(rows: CardModel[][], maxRowWidth: number): CardModel[][] {
    const normalizedRows = rows.map((row) => [...row]);

    if (maxRowWidth > 0) {
      this.moveOverflowCards(normalizedRows, maxRowWidth);
    }

    return this.updateCardPositions(normalizedRows);
  }

  private updateCardPositions(rows: CardModel[][]): CardModel[][] {
    return rows
      .filter((row) => row.length > 0)
      .map((row, rowIndex) =>
        row.map((card, indexPosition) => ({
          ...card,
          indexPosition: indexPosition + 1,
          rowPosition: rowIndex + 1,
        })),
      );
  }

  private moveOverflowCards(rows: CardModel[][], maxRowWidth: number): void {
    for (let rowIndex = 0; rowIndex < rows.length; rowIndex++) {
      const row = rows[rowIndex];

      while (row.length > 1 && this.getRowWidth(row) > maxRowWidth) {
        const overflowCard = row.pop();
        if (!overflowCard) break;

        rows[rowIndex + 1] = rows[rowIndex + 1] ?? [];
        rows[rowIndex + 1].unshift(overflowCard);
      }
    }
  }

  private getRowWidth(row: CardModel[]): number {
    const cardsWidth = row.reduce((width, card) =>
        width + card.settings!.width, 0,
    );

    return cardsWidth + Math.max(row.length - 1, 0) * 32;
  }

  refresh(): void {
    this.#api.get().pipe(take(1))
      .subscribe((cards) =>
        this.cardsSubject.next(cards));
  }
}
