import { AfterViewInit, Directive, ElementRef, EventEmitter, HostListener, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';

@Directive({
  selector: '[appDialogWindow]',
  standalone: true,
  exportAs: 'appDialogWindow',
})
export class DialogDirective implements AfterViewInit, OnChanges {
  private el = inject<ElementRef<HTMLDialogElement>>(ElementRef);


  @Input() dialogId = 0;
  @Input() id = 0;
  @Input() open = false;
  @Output() openChange = new EventEmitter<number>();

  ngAfterViewInit(): void {
    this.syncDialog();
    this.el.nativeElement.addEventListener('close', () =>
      this.openChange.emit(this.dialogId)
    );
    this.el.nativeElement.addEventListener('cancel', () =>
      this.openChange.emit(this.dialogId)
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] || changes['dialogId'] || changes['id']) {
      this.syncDialog();
    }
  }

  private syncDialog(): void {
    const dialog = this.el.nativeElement;
    if (this.open && this.dialogId === this.id && !dialog.open) {
      dialog.showModal();
    }
    if ((!this.open || this.dialogId !== this.id) && dialog.open) {
      dialog.close();
    }
  }

  close(): void {
    this.el.nativeElement.close();
    this.openChange.emit(this.dialogId);
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
