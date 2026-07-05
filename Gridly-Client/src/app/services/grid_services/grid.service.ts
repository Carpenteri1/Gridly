import {inject, Injectable, Signal, signal} from "@angular/core";
import {BehaviorSubject, firstValueFrom, Observable, take} from "rxjs";
import {RowColumnModel} from "../../models/rowColumn.Model";
import {RowColumnEndpointService} from "../endpoint_services/rowColumn.endpoint.service";
import {toSignal} from "@angular/core/rxjs-interop";
import {CardModel} from "../../models/card.Model";

@Injectable({providedIn: 'root'})
export class GridService {
  private readonly cardGap = 32;
  private readonly defaultCardWidth = 250;
  private readonly RowColumnSubject = new BehaviorSubject<RowColumnModel[]>([]);
  private readonly _editMode = signal(false);
  private availableRowWidth = 0;

  readonly rows$: Observable<RowColumnModel[]>;
  readonly inEditMode = this._editMode.asReadonly();
  readonly currentRowColumns: Signal<RowColumnModel[]>;

  #api = inject(RowColumnEndpointService);

  constructor() {
    this.rows$ = this.RowColumnSubject.asObservable();
    this.currentRowColumns = toSignal(this.rows$, { initialValue: [] as RowColumnModel[] });
    this.refresh();
  }

  private batchSave$ = (rowColumn: RowColumnModel[]) => this.#api.batchSave(rowColumn);

  batchSave = (rowColumn: RowColumnModel[]) =>
    firstValueFrom(this.batchSave$(this.normalizeRows(rowColumn))).then(() => this.refresh());

  toggleEdit = () => this._editMode.update((value) => !value);

  refresh(): void {
    this.#api.get().pipe(take(1))
      .subscribe((rowColumns) =>
        this.RowColumnSubject.next(this.normalizeRows(rowColumns ?? [])));
  }

  setRowsForView(rows: RowColumnModel[]): void {
    this.RowColumnSubject.next(this.normalizeRows(rows));
  }

  setAvailableRowWidth(width: number): void {
    if (width <= 0 || width === this.availableRowWidth) return;

    this.availableRowWidth = width;
  }

  addCardToFirstAvailableRow(card: CardModel): void {
    const rows = this.cloneRows(this.currentRowColumns());
    const newCard = {
      ...card,
      id: 0,
      settings: {
        width: card.settings?.width ?? this.defaultCardWidth,
        height: card.settings?.height ?? this.defaultCardWidth,
        imageHidden: card.settings?.imageHidden ?? false,
        titleHidden: card.settings?.titleHidden ?? false,
      },
    };

    const targetRow = rows.find((row) => this.cardFits(row.cards, newCard));

    if (targetRow) {
      targetRow.cards = [...targetRow.cards, {
        ...newCard,
        rowPosition: targetRow.rowPosition,
        indexPosition: targetRow.cards.length + 1,
      }];
    } else {
      rows.push({
        id: 0,
        rowPosition: rows.length + 1,
        rowWidth: 0,
        cards: [{
          ...newCard,
          rowPosition: rows.length + 1,
          indexPosition: 1,
        }],
      });
    }

    this.RowColumnSubject.next(this.normalizeRows(rows));
  }

  removeCardFromView(cardToRemove: CardModel): void {
    const removed = this.currentRowColumns()
      .map((row) => ({
        ...row,
        cards: row.cards.filter((card) =>
          !this.isSameCard(card, cardToRemove)),
      }));

    this.RowColumnSubject.next(this.normalizeRows(removed));
  }

  updateCardInView(cardToUpdate: CardModel, updatedCard: CardModel): void {
    const updatedRows = this.currentRowColumns()
      .map((row) => ({
        ...row,
        cards: row.cards.map((card) =>
          this.isSameCard(card, cardToUpdate) ? { ...card, ...updatedCard } : card),
      }));

    this.RowColumnSubject.next(this.normalizeRows(updatedRows));
  }

  normalizeRows(rows: RowColumnModel[]): RowColumnModel[] {
    return this.cloneRows(rows)
      .filter((row) => row.cards.length > 0)
      .map((row, rowIndex) => ({
        ...row,
        rowPosition: rowIndex + 1,
        rowWidth: row.rowWidth ?? 0,
        cards: row.cards.map((card, cardIndex) => ({
          ...card,
          rowColumnId: row.id,
          rowPosition: rowIndex + 1,
          indexPosition: cardIndex + 1,
        })),
      }));
  }

  private reflowRows(rows: RowColumnModel[]): RowColumnModel[] {
    if (this.availableRowWidth <= 0) return this.normalizeRows(rows);

    const sortedRows = this.normalizeRows(rows);
    const reflowedRows = sortedRows
      .flatMap((row) => row.cards)
      .reduce((result, card) => {
        const currentRow = result[result.length - 1];

        if (currentRow && this.cardFits(currentRow.cards, card)) {
          currentRow.cards = [...currentRow.cards, card];
        } else {
          const rowIndex = result.length;
          const existingRow = sortedRows[rowIndex];
          result.push({
            id: existingRow?.id ?? 0,
            rowPosition: rowIndex + 1,
            rowWidth: existingRow?.rowWidth ?? 0,
            cards: [card],
          });
        }

        return result;
      }, [] as RowColumnModel[]);

    return this.normalizeRows(reflowedRows);
  }

  private cardFits(rowCards: CardModel[], card: CardModel): boolean {
    if (this.availableRowWidth <= 0) return true;

    const currentWidth = this.getCardsWidth(rowCards);
    const addedGap = rowCards.length > 0 ? this.cardGap : 0;
    return currentWidth + addedGap + this.getCardWidth(card) <= this.availableRowWidth;
  }

  private getCardsWidth(cards: CardModel[]): number {
    const cardsWidth = cards.reduce((width, card) => width + this.getCardWidth(card), 0);
    const gapsWidth = Math.max(cards.length - 1, 0) * this.cardGap;
    return cardsWidth + gapsWidth;
  }

  private getCardWidth(card: CardModel): number {
    return card.settings?.width ?? this.defaultCardWidth;
  }

  private cloneRows(rows: RowColumnModel[]): RowColumnModel[] {
    return rows.map((row) => ({
      ...row,
      cards: [...row.cards],
    }));
  }

  private isSameCard(card: CardModel, other: CardModel): boolean {
    return other.id === 0 ? card === other : card.id === other.id;
  }

}
