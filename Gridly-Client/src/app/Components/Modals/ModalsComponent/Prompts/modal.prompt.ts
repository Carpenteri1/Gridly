import {Component} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {ModalViewModel} from "../../../../Models/ModalView.Model";
import {FormsModule} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {ModalService} from "../../../../Services/modal.service";
import {MatHeaderRow} from "@angular/material/table";
import {MatTooltip} from "@angular/material/tooltip";

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
      MatHeaderRow,
      MatTooltip]
})
export class ModalPrompt {
  public modalModel!: ModalViewModel;
  constructor(protected modalService: ModalService){}
}
