import {
  AfterViewInit,
  Directive,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';

@Directive({
  selector: '[modalWindow]',
  standalone: true,
  exportAs: 'modalWindow',
})
export class ModalDirective implements AfterViewInit, OnChanges {
  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Input() open: boolean = false;
  @Output() openChange = new EventEmitter<number>();

  constructor(private el: ElementRef<HTMLDialogElement>) {}

  ngAfterViewInit(): void {
    this.syncDialog();
    this.el.nativeElement.addEventListener('close', () =>
      this.openChange.emit(this.modalId)
    );
    this.el.nativeElement.addEventListener('cancel', () =>
      this.openChange.emit(this.modalId)
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] || changes['modalId'] || changes['id']) {
      this.syncDialog();
    }
  }

  private syncDialog(): void {
    const dialog = this.el.nativeElement;
    if (this.open && this.modalId === this.id && !dialog.open) {
      dialog.showModal();
    }
    if ((!this.open || this.modalId !== this.id) && dialog.open) {
      dialog.close();
    }
  }

  close(): void {
    this.el.nativeElement.close();
      this.openChange.emit(this.modalId);
  }

  onBackdropClick(ev: MouseEvent): void {
    if (ev.target === this.el.nativeElement) {
      this.close();
    }
  }
}
