import {Component} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {IModalsModel} from "../../../Models/IModals.Model";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatDivider} from "@angular/material/divider";
import {ModalService} from "../../../Services/modal.service";
import {MatSelect} from "@angular/material/select";
import {MatOption} from "@angular/material/core";

@Component({
  selector: 'modals-component',
  templateUrl: './modals.component.html',
  standalone: true,
  imports:
    [MatDialogClose,
    MatDialogContent,
    MatDialogActions,
    MatFormField,
    MatLabel,
    MatFormField,
    FormsModule,
    MatInput,
    MatDialogTitle,
    MatButton,
    MatDivider,
    MatOption,
    MatSelect,
    MatOption,
    MatSelect]
})
export class ModalsComponent {
  public viewModel!: IModalsModel;
  constructor(public modalService: ModalService){}
}
