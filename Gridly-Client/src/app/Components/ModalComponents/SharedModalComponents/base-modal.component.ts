import { ViewChild, Directive, AfterViewInit } from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { ModalBehaviorService } from '../../../Services/modal-behavior.service';
import { TextStringsUtil } from '../../../Constants/text.strings.util';
import { ModalViewModel } from '../../../Models/ModalView.Model';

@Directive()
export abstract class BaseModalComponent implements AfterViewInit {
  @ViewChild(ModalDirective) modalDirective!: ModalDirective;

  protected readonly TextStringsUtil = TextStringsUtil;

  constructor(protected modalBehavior: ModalBehaviorService) {}

  ngAfterViewInit(): void {
    // Override in child components if needed
  }

  close(): void {
    this.modalDirective?.close();
  }

  onBackdropClick(ev: MouseEvent): void {
    this.modalDirective?.onBackdropClick(ev);
  }

  async submit(modalType: ModalViewModel): Promise<void> {
    await this.modalBehavior.submit(modalType);
    this.close();
  }

  onFileUpload(event: any) {
    return this.modalBehavior.onFileUpload(event);
  }

  resetImageData(): void {
    this.modalBehavior.resetImageData();
  }

  get isOpen(): boolean {
    return this.modalDirective?.open ?? false;
  }
}

