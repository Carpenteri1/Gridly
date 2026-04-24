import { Component, inject, Renderer2 } from '@angular/core';
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
  private render = inject(Renderer2);

  #componentService = inject(ComponentService);
  #gridService = inject(GridService);

  components$ = this.#componentService.components$;

  inEditMode!: boolean;

  constructor() {
    this.inEditMode = this.#gridService.getEditMode();
    this.SetLayout();
  }

  protected Drop(event: CdkDragDrop<ComponentModel[]>): void {
    if (!this.inEditMode) return;
    moveItemInArray(event.item.data, event.previousIndex, event.currentIndex);
  }

  protected SetLayout(): void {
    /*for (const item of this.components) {
      const el = document.getElementById(item.id.toString());
      if(el) {
        this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
        this.render.setStyle(el, 'width', item.componentSettings?.width + 'px');
        this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
      }
    }*/
  }
}
