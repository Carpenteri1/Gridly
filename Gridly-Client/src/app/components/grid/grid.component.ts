import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardService } from "../../services/card_services/card.service";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid_services/grid.service';
import { CardComponent } from '../card/card.component';
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

  readonly cards$ = this.#cardService.cards$;
  readonly inEditMode = this.#gridService.editMode;

  protected Drop(event: CdkDragDrop<CardModel[]>): void {
    if (!this.inEditMode()) return;
    moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    event.container.data.forEach((card, index) => {
      card.indexPosition = index + 1;
    });
  }
}
