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
    //TODO needs to remove a row if a row has only one card thats remove
    // Update all rows to have the correct index position
    if (!this.editActive()) return;

    const droppedCard = event.item.data as CardModel;

    if(rows.length >= newRowPosition)
    {
      const updatedRows = rows.map(row => {
        const cards = [...row.cards];
        const [movedCard] = cards.splice(event.previousIndex, 1);

        cards.splice(event.currentIndex, 0, movedCard);

        return {
          ...row,
          cards: cards.map((card, index) => ({
            ...card,
            rowPosition: newRowPosition,
            indexPosition: index,
          })),
        };
      });

      this.#gridService.setRowsForView(updatedRows);
      return;
    }

    droppedCard.indexPosition = 1;

    const newRow: RowColumnModel = {
      id: 0,
      rowPosition: newRowPosition,
      cards: [droppedCard],
    };

    const rowsWithoutCard = rows.map(row => ({...row,
      cards: row.cards.filter(card => card.id !== droppedCard.id),
    }));

    this.#gridService.setRowsForView([...rowsWithoutCard, newRow]);
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
