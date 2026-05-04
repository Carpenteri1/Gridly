import { Directive, ViewChild, inject } from '@angular/core';
import { DialogService } from '../services/dialog_services/dialog.service';
import { TextStringsUtil } from '../constants/text.strings.util';
import { DialogDirective } from './dialog.directive';

@Directive()
export abstract class BaseDialogComponent {
  @ViewChild(DialogDirective) modalDirective!: DialogDirective;

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly dialogService = inject(DialogService);


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
    return this.dialogService.onFileUpload(event);
  }

  resetImageData(): void {
    this.dialogService.resetImageData();
  }

  get isOpen(): boolean {
    return this.modalDirective?.open ?? false;
  }
}
