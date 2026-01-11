import { ViewChild, Directive } from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { ModalService } from '../../../Services/modal.service';
import { TextStringsUtil } from '../../../Constants/text.strings.util';
import { ModalViewModel } from '../../../Models/ModalView.Model';

@Directive()
export abstract class BaseModalComponent {
  @ViewChild(ModalDirective) modalDirective!: ModalDirective;

  protected readonly TextStringsUtil = TextStringsUtil;

  constructor(protected modalService: ModalService) {}


  close(): void {
    this.modalDirective?.close();
  }

  onBackdropClick(ev: MouseEvent): void {
    this.modalDirective?.onBackdropClick(ev);
  }

  async submit(modalType: ModalViewModel): Promise<void> {
    await this.modalService.submit(modalType);
    this.close();
  }

  onFileUpload(event: any) {
    return this.modalService.onFileUpload(event);
  }

  resetImageData(): void {
    this.modalService.resetImageData();
  }

  get isOpen(): boolean {
    return this.modalDirective?.open ?? false;
  }
}

