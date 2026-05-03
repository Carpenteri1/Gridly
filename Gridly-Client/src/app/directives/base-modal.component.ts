import { Directive, ViewChild, inject } from '@angular/core';
import { ModalDirective } from './modal.directive';
import { ModalService } from '../services/dialog.service';
import { TextStringsUtil } from '../constants/text.strings.util';

@Directive()
export abstract class BaseModalComponent {
  @ViewChild(ModalDirective) modalDirective!: ModalDirective;

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly modalService = inject(ModalService);


  close(): void {
    this.modalDirective?.close();
  }

  onBackdropClick(ev: MouseEvent): void {
    this.modalDirective?.onBackdropClick(ev);
  }

  async submit(): Promise<void> {
    ///await this.modalService.submit(modalType);
    //TODO call delete or edit from here or make new method that can handle that
    this.close();
  }

  onFileUpload(event: Event): Promise<unknown> {
    return this.modalService.onFileUpload(event);
  }

  resetImageData(): void {
    this.modalService.resetImageData();
  }

  get isOpen(): boolean {
    return this.modalDirective?.open ?? false;
  }
}
