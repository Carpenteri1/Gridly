import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentService } from "../../services/component.service";
import { CardComponent } from "../card/card.component";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { CardModel } from '../../models/card.Model';
import { GridService } from '../../services/grid.service';
@Component({
  selector: 'app-grid',
  imports: [CommonModule, CdkDropList, CdkDrag, CardComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent {
  #componentService = inject(ComponentService);
  #gridService = inject(GridService);

  readonly components$ = this.#componentService.components$;
  readonly inEditMode = this.#gridService.editMode;

  protected Drop(event: CdkDragDrop<CardModel[]>): void {
    if (!this.inEditMode()) return;
    moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
  }
}
