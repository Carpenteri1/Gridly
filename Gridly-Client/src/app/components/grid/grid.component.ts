import { Component, ElementRef, ViewChild, inject } from '@angular/core';
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

export class GridComponent {
  #gridService = inject(GridService);

  @ViewChild('gridLayout') private gridLayout?: ElementRef<HTMLElement>;

  protected readonly rows$ = this.#gridService.rows$;
  protected readonly editActive = this.#gridService.inEditMode;
  protected emptyRow: CardModel[] = [];

  protected Drop(event: CdkDragDrop<CardModel[]>, rows: RowColumnModel[], newRowPosition: number): void {
    if (!this.editActive()) return;

    const droppedCard = event.item.data as CardModel;
    const sourceRow = rows.find(row => row.cards.some(card => card.id === droppedCard.id));
    const rowsWithoutDroppedCard = rows.map(row => ({
      ...row,
      cards: row.cards.filter(card => card.id !== droppedCard.id),
    }));

    const targetRowIndex = rowsWithoutDroppedCard.findIndex(row => row.rowPosition === newRowPosition);

    if (targetRowIndex >= 0) {
      const targetCards = [...rowsWithoutDroppedCard[targetRowIndex].cards];
      targetCards.splice(event.currentIndex, 0, droppedCard);
      rowsWithoutDroppedCard[targetRowIndex] = {
        ...rowsWithoutDroppedCard[targetRowIndex],
        cards: targetCards,
      };
    } else {
      rowsWithoutDroppedCard.push({
        id: 0,
        rowPosition: newRowPosition,
        cards: [droppedCard],
      });
    }

    this.#gridService.setRowsForView(this.updateRowPositions(rowsWithoutDroppedCard, sourceRow));
  }

  private updateRowPositions(rows: RowColumnModel[], sourceRow?: RowColumnModel): RowColumnModel[] {
    return rows
      .filter(row => row.cards.length > 0 || row.id !== sourceRow?.id)
      .map((row, rowIndex) => ({
        ...row,
        rowPosition: rowIndex + 1,
        cards: row.cards.map((card, cardIndex) => ({
          ...card,
          rowColumnId: row.id,
          rowPosition: rowIndex + 1,
          indexPosition: cardIndex,
        })),
      }));
  }

  protected RowId(rowIndex: number): string {
    return `card-row-${rowIndex}`;
  }

  protected RowIds(rows: RowColumnModel[]): string[] {
    return [
      ...rows.map(row => this.RowId(row.rowPosition!)),
      this.RowId(rows.length + 1),
    ];
  }

  private getRowIndex(rowId: string): number {
    return Number(rowId.replace('card-row-', ''));
  }
}
