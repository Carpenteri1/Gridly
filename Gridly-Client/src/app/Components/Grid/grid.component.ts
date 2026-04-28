import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentService } from "../../Services/component.service";
import { ItemComponent } from "../Item/item.component";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { ComponentModel } from '../../Models/Component.Model';
import { GridService } from '../../Services/grid.service';
@Component({
  selector: 'app-grid',
  imports: [CommonModule, CdkDropList, CdkDrag, ItemComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent {
  #componentService = inject(ComponentService);
  #gridService = inject(GridService);

  readonly components$ = this.#componentService.components$;
  readonly inEditMode = this.#gridService.editMode;

  protected Drop(event: CdkDragDrop<ComponentModel[]>): void {
    if (!this.inEditMode()) return;
    moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
  }
}
