import {Component, ElementRef, OnInit, ViewChild} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {ModalViewModel} from "../../../../Models/ModalView.Model";
import {FormsModule} from "@angular/forms";
import {ModalService} from "../../../../Services/modal.service";

@Component({
  templateUrl: './modal.component-form.html',
  standalone: true,
  imports:
    [FormsModule, MatDialogActions, MatDialogContent, MatDialogTitle, MatDialogClose]
})
export class ModalComponentForm implements OnInit {
  public modalModel!: ModalViewModel;
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  constructor(protected modalService: ModalService){}
  ngOnInit() {
    this.modalService.resetFile$.subscribe(() => {
      this.fileInputRef?.nativeElement && (this.fileInputRef.nativeElement.value = '');
    });
  }
  protected ResetImageInput(): void {
    this.modalService.ResetImageData();
  }
  protected OnFileUpload(event:any){
    this.modalModel.component.iconData = this.modalService.OnFileUpload(event);
  }
}
