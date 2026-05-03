import { AfterViewInit, Directive, ElementRef, EventEmitter, HostListener, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';

@Directive({
  selector: '[appModalWindow]',
  standalone: true,
  exportAs: 'appModalWindow',
})
export class ModalDirective implements AfterViewInit, OnChanges {
  private el = inject<ElementRef<HTMLDialogElement>>(ElementRef);


  @Input() modalId = 0;
  @Input() id = 0;
  @Input() open = false;
  @Output() openChange = new EventEmitter<number>();

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

  save(): void {
    this.close();
  }

  @HostListener('click', ['$event'])
  onBackdropClick(ev: MouseEvent): void {
    if (ev.target === this.el.nativeElement) {
      this.close();
    }
  }
}
