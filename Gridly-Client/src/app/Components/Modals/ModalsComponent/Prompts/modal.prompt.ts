import {Component, OnInit} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {ModalViewModel} from "../../../../Models/ModalView.Model";
import {FormsModule} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {ModalService} from "../../../../Services/modal.service";
import {MatHeaderRow} from "@angular/material/table";
import {ComponentService} from "../../../../Services/component.service";
import {MapComponentData} from "../../../../Utils/componentModal.factory";

@Component({
  templateUrl: './modal.prompt.html',
  standalone: true,
  imports:
    [MatDialogClose,
      MatDialogContent,
      MatDialogActions,
      FormsModule,
      MatDialogTitle,
      MatButton,
      MatHeaderRow]
})
export class ModalPrompt implements OnInit{
  protected modalModel!: ModalViewModel;
  constructor(protected modalService: ModalService,protected componentService: ComponentService) {}

  ngOnInit(): void {
    this.componentService.Component = MapComponentData(this.modalModel.component)
  }
}
