import { Component, ElementRef, HostListener, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardService } from "../../services/card_services/card.service";
import { CdkDrag, CdkDragDrop, CdkDropList } from "@angular/cdk/drag-drop";
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { CardComponent } from '../card/card.component';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
@Component({
  selector: 'app-grid',
  imports: [CommonModule, CdkDropList, CdkDrag, CardComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent {
  #cardService = inject(CardService);
  #gridService = inject(GridService);

  @ViewChild('gridLayout') private gridLayout?: ElementRef<HTMLElement>;

  private readonly gridWidthSubject = new BehaviorSubject<number>(0);
  protected readonly emptyRowData: CardModel[] = [];
  protected readonly rows$ = combineLatest([this.#cardService.cards$, this.gridWidthSubject]).pipe(
    map(([cards, maxRowWidth]) => this.#cardService.toRows(cards, maxRowWidth)),
  );
  protected readonly editActive = this.#gridService.inEditMode;

  protected Drop(event: CdkDragDrop<CardModel[]>, rows: CardModel[][], rowIndex: number): void {
    if (!this.editActive()) return;

    const updatedRows = rows.map((row) => [...row]);
    const previousRowIndex = this.getRowIndex(event.previousContainer.id);
    const movedCard = event.item.data as CardModel;

    if (previousRowIndex === rowIndex) {
      const row = updatedRows[rowIndex];
      row.splice(event.previousIndex, 1);
      row.splice(event.currentIndex, 0, movedCard);
    } else {
      updatedRows[previousRowIndex]?.splice(event.previousIndex, 1);
      updatedRows[rowIndex] = updatedRows[rowIndex] ?? [];
      updatedRows[rowIndex].splice(event.currentIndex, 0, movedCard);
    }

    this.#cardService.setRows(updatedRows, this.gridWidthSubject.value);
  }

  protected RowId(rowIndex: number): string {
    return `card-row-${rowIndex}`;
  }

  protected RowIds(rows: CardModel[][]): string[] {
    return [...rows, []].map((_, rowIndex) => this.RowId(rowIndex));
  }

  private getRowIndex(rowId: string): number {
    return Number(rowId.replace('card-row-', ''));
  }
}
