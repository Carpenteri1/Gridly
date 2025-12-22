import {Component, Input, Output, EventEmitter,ViewChild, ElementRef, OnChanges, SimpleChanges, AfterViewInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TextStringsUtil} from "../../../Constants/text.strings.util";
import {ComponentModel} from "../../../Models/Component.Model";
import {IconModel} from "../../../Models/Icon.Model";
import { ModalViewModel } from '../../../Models/ModalView.Model';
import { ModalService } from '../../../Services/modal.service';

@Component({
  selector: 'edit-widget-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-widget-modal.component.html',
  styleUrls: ['./edit-widget-modal.component.css']
})
export class EditWidgetModalComponent implements AfterViewInit, OnChanges
{ 
   public modalModel!: ModalViewModel;
   constructor(protected modalService: ModalService){}


  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Input() open = false;
  @Input() component: ComponentModel = {
    id: 0,
    indexPosition: 0,
    name: '',
    url: ''
  };
  @Input() icons: IconModel[] = [];
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  @Output() openChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<{component: ComponentModel, selectedIconId: number | null}>();
  protected readonly TextStringsUtil = TextStringsUtil;

  selectedIconId: number | null = null;

  @ViewChild('dlg', { static: true }) dlgRef!: ElementRef<HTMLDialogElement>;
WidgetType: any;

  ngAfterViewInit(): void {
    this.syncDialog();
    this.dlgRef.nativeElement.addEventListener('close', () => this.openChange.emit(false));
    this.dlgRef.nativeElement.addEventListener('cancel', () => this.openChange.emit(false)); // Esc
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.modalId === this.id)
    {
      if (!this.dlgRef) return;
      if (changes['open']) this.syncDialog();
      if (changes['component'] && this.component) {
        const iconData = this.component.iconData;
        this.selectedIconId = (iconData && 'id' in iconData && iconData.id) ? iconData.id : null;
      }
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

  onSave() {
    this.save.emit({
      component: this.component,
      selectedIconId: this.selectedIconId
    });
    this.close();
  }

  protected OnFileUpload(event:any){
    this.modalModel.component.iconData = this.modalService.OnFileUpload(event);
  }
  protected ResetImageInput(): void {
    this.modalService.resetFile$.subscribe(() => {
      this.fileInputRef.nativeElement.value = '';
    });
    this.modalService.ResetImageData();
  }

}
