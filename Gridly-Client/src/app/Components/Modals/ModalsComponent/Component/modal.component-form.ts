import {Component, ElementRef, ViewChild} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {ModalViewModel} from "../../../../Models/ModalView.Model";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatDivider} from "@angular/material/divider";
import {ModalService} from "../../../../Services/modal.service";
import {MatSelect} from "@angular/material/select";
import {MatOption} from "@angular/material/core";
import {NgClass} from "@angular/common";

@Component({
  templateUrl: './modal.component-form.html',
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
      MatSelect,
      NgClass]
})
export class ModalComponentForm {
  public modalModel!: ModalViewModel;
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  constructor(protected modalService: ModalService){}

  protected OnFileUpload(event:any){
    this.modalModel.component.iconData = this.modalService.OnFileUpload(event);
  }
  protected ResetImageInput(): void {
    this.modalService.resetFile$.subscribe(() => {
      this.fileInputRef.nativeElement.value = '';
    });
    this.modalService.ResetImageData();
  }
}
