import {inject, Injectable, Signal, signal} from "@angular/core";
import {BehaviorSubject, firstValueFrom, Observable, take} from "rxjs";
import {RowColumnModel} from "../../models/rowColumn.Model";
import {RowColumnEndpointService} from "../endpoint_services/rowColumn.endpoint.service";
import {toSignal} from "@angular/core/rxjs-interop";
import {CardModel} from "../../models/card.Model";

@Injectable({providedIn: 'root'})
export class GridService {
  private readonly RowColumnSubject = new BehaviorSubject<RowColumnModel[]>([]);
  private readonly _editMode = signal(false);

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

  addCardToFirstAvailableRow(card: CardModel): void {
    const rows = this.cloneRows(this.currentRowColumns());
    const newCard = {
      ...card,
      id: 0,
      settings: {
        width: card.settings?.width ?? 0,
        height: card.settings?.height ?? 0,
        imageHidden: card.settings?.imageHidden,
        titleHidden: card.settings?.titleHidden,
      },
    };

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
