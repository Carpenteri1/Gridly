import {Component, Input, Output, EventEmitter,ViewChild, ElementRef, OnChanges, SimpleChanges, AfterViewInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TextStringsUtil} from "../../../Constants/text.strings.util";
import {WidgetOptionsModal} from "../../../Models/WidgetOptionsModal";

@Component({
  selector: 'add-widget-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './add-widget-modal.component.html',
  styleUrls: ['./add-widget-modal.component.css']
})
export class AddWidgetModalComponent implements AfterViewInit, OnChanges {
  @Input() open = false;
  @Input() widgetOptions: WidgetOptionsModal[] = [];

  @Output() openChange = new EventEmitter<boolean>();
  @Output() select = new EventEmitter<string>();
  protected readonly TextStringsUtil = TextStringsUtil;

  @ViewChild('dlg', { static: true }) dlgRef!: ElementRef<HTMLDialogElement>;

  ngAfterViewInit(): void {
    this.syncDialog();
    this.dlgRef.nativeElement.addEventListener('close', () => this.openChange.emit(false));
    this.dlgRef.nativeElement.addEventListener('cancel', () => this.openChange.emit(false)); // Esc
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.dlgRef) return;
    if (changes['open']) this.syncDialog();
  }

  private syncDialog() {
    const dlg = this.dlgRef.nativeElement;
    if (this.open && !dlg.open) dlg.showModal();
    if (!this.open && dlg.open) dlg.close();
  }

  close() {
    this.dlgRef.nativeElement.close();
    this.openChange.emit(false);
  }

  onBackdropClick(ev: MouseEvent) {
    if (ev.target === this.dlgRef.nativeElement) this.close();
  }

  onSelect(type: string) {
    this.select.emit(type);
    this.close();
  }

}
