import {AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild} from "@angular/core";
import {ModalViewModel} from "../../../Models/ModalView.Model";
import {FormsModule} from "@angular/forms";
import {ModalService} from "../../../Services/modal.service";
import {ComponentService} from "../../../Services/component.service";
import { TextStringsUtil } from "../../../Constants/text.strings.util";
import { WidgetType } from "../../../Types/widget.type.enum";

@Component({
  selector: 'prompt-modal',
  templateUrl: './prompt-modal.component.html',
  standalone: true,
  imports:
    [FormsModule]
})
export class PromptModalComponent implements AfterViewInit, OnChanges
{
  protected modalModel!: ModalViewModel;
  constructor(protected modalService: ModalService, protected componentService: ComponentService) {}

  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Input() open = false;

  @Output() openChange = new EventEmitter<boolean>();
  @Output() select = new EventEmitter<WidgetType>();
  protected readonly TextStringsUtil = TextStringsUtil;

  @ViewChild('dlg', { static: true }) dlgRef!: ElementRef<HTMLDialogElement>;

  ngAfterViewInit(): void {
    this.syncDialog();
    this.dlgRef.nativeElement.addEventListener('close', () => this.openChange.emit(false));
    this.dlgRef.nativeElement.addEventListener('cancel', () => this.openChange.emit(false));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.modalId === this.id)
    {
      if (!this.dlgRef) return;
      if (changes['open']) this.syncDialog();
    }
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

  onSelect(type: WidgetType) {
    this.select.emit(type);
    this.close();
  }
}
