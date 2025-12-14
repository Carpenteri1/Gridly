import {Component, Input, Output, EventEmitter,ViewChild, ElementRef, OnChanges, SimpleChanges, AfterViewInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TextStringsUtil} from "../../../Constants/text.strings.util";

@Component({
  selector: 'edit-widget-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './edit-widget-dialog.component.html',
  styleUrls: ['./edit-widget-dialog.component.css']
})
export class EditWidgetDialogComponent implements AfterViewInit, OnChanges
{
  @Input() id: number = 0;
  @Input() open = false;

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
    debugger;
    if (ev.target === this.dlgRef.nativeElement) this.close();
  }

  onSelect(type: string) {
    this.select.emit(type);
    this.close();
  }
}
