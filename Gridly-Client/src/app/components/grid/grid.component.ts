import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDrag, CdkDragDrop, CdkDropList } from "@angular/cdk/drag-drop";
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { CardComponent } from '../card/card.component';
import { RowColumnModel } from "../../models/rowColumn.Model";

@Component({
  selector: 'app-grid',
  imports: [CommonModule, CdkDropList, CdkDrag, CardComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent implements AfterViewInit, OnDestroy {
  #gridService = inject(GridService);

  @ViewChild('gridLayout') private gridLayout?: ElementRef<HTMLElement>;
  private resizeObserver?: ResizeObserver;

  protected readonly rows$ = this.#gridService.rows$;
  protected readonly editActive = this.#gridService.inEditMode;
  protected emptyRow: CardModel[] = [];

  //TODO look over this class. Somethings fishy in the structure
  ngAfterViewInit(): void {
    this.resizeObserver = new ResizeObserver(() => this.updateAvailableRowWidth());
    this.resizeObserver.observe(this.gridLayout!.nativeElement);
    this.updateAvailableRowWidth();
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
  }

  protected Drop(event: CdkDragDrop<CardModel[]>, rows: RowColumnModel[], newRowPosition: number): void {
    if (!this.editActive()) return;

    const droppedCard = event.item.data as CardModel;
    const nextRows = rows.map(row => ({
      ...row,
      cards: row.cards.filter(card => !this.isSameCard(card, droppedCard)),
    }));
    const targetRow = nextRows.find(row => row.rowPosition === newRowPosition);

    if (targetRow) {
      targetRow.cards = [
        ...targetRow.cards.slice(0, event.currentIndex),
        droppedCard,
        ...targetRow.cards.slice(event.currentIndex),
      ];
    } else {
      nextRows.push({
        id: 0,
        rowPosition: newRowPosition,
        cards: [droppedCard],
      });
    }

    this.#gridService.setRowsForView(this.#gridService.normalizeRows(nextRows));
  }

  protected DropToNewRow(event: CdkDragDrop<CardModel[]>, rows: RowColumnModel[], newRowPosition: number): void {
    if (!this.editActive()) return;

    const droppedCard = event.item.data as CardModel;
    const rowsWithoutDroppedCard = rows.map(row => ({
      ...row,
      cards: row.cards.filter(card => !this.isSameCard(card, droppedCard)),
    }));

    rowsWithoutDroppedCard.splice(newRowPosition - 1, 0, {
      id: 0,
      rowPosition: newRowPosition,
      rowWidth: 0,
      cards: [droppedCard],
    });

    this.#gridService.setRowsForView(this.#gridService.normalizeRows(rowsWithoutDroppedCard));
  }

  protected RowId(rowIndex: number): string {
    return `card-row-${rowIndex}`;
  }

  protected InsertRowId(rowIndex: number): string {
    return `card-row-insert-${rowIndex}`;
  }

  protected RowIds(rows: RowColumnModel[]): string[] {
    return [
      ...rows.map(row => this.RowId(row.rowPosition!)),
      ...Array.from({ length: rows.length + 1 }, (_, index) => this.InsertRowId(index + 1)),
      this.RowId(rows.length + 1),
    ];
  }

  protected InsertRowPositions(rows: RowColumnModel[]): number[] {
    return Array.from({ length: rows.length + 1 }, (_, index) => index + 1);
  }

  protected CardTrack(card: CardModel, index: number): string {
    return card.id > 0 ? card.id.toString() : `new-${card.rowPosition}-${card.indexPosition}-${index}`;
  }

  protected CardDomId(row: RowColumnModel, card: CardModel): string {
    return card.id > 0 ? card.id.toString() : `new-card-${row.rowPosition}-${card.indexPosition}`;
  }

  private updateAvailableRowWidth(): void {
    const row = this.gridLayout?.nativeElement.querySelector('.grid-row') as HTMLElement | null;
    const element = row ?? this.gridLayout?.nativeElement;
    if (!element) return;

    const styles = getComputedStyle(element);
    const horizontalPadding = parseFloat(styles.paddingLeft) + parseFloat(styles.paddingRight);
    this.#gridService.setAvailableRowWidth(element.clientWidth - horizontalPadding);
  }

  private isSameCard(card: CardModel, other: CardModel): boolean {
    return other.id === 0 ? card === other : card.id === other.id;
  }
}
