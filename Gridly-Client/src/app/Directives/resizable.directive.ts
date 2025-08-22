import {Directive, ElementRef, HostListener, Input, Renderer2} from '@angular/core';
import {ComponentSettingsModel} from "../Models/ComponentSettings.Model";
import {ComponentModel} from "../Models/Component.Model";
import {ComponentService} from "../Services/component.service";
import {EndPointType} from "../Types/endPoint.type.enum";
import {MapComponentData} from "../Utils/componentModal.factory";

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective {
  @Input() itemResizing!: ComponentModel;
  @Input() canResize!: boolean;
  private itemId!: string;

  constructor(private el: ElementRef,
              private renderer: Renderer2,
              private componentService: ComponentService) {}

  @HostListener('document:pointermove', ['$event'])
  OnMouseMove(event: MouseEvent): void {
    if(!this.canResize) return;
    if(this.itemId !== "") {
      this.itemResizing = MapComponentData(this.itemResizing,{id: Number(this.itemId)});
      this.itemResizing = this.componentService.CallEndpoint(EndPointType.GetById,undefined, this.itemResizing) as ComponentModel;
      this.ResizeComponent(event.clientX, event.clientY);
    }
  }

  @HostListener('pointerdown', ['$event'])
  OnLeftMouseButtonPress(event: MouseEvent): void {
    this.HideCursor();
    const target = event.target as HTMLElement;
    this.itemId = target.id;
    if(this.itemId === ""){
      this.itemId = target.id;
    }
  }

  @HostListener('document:pointerup', ['$event'])
  OnLeftMouseButtonRelease(){
    this.ShowCursor();
    this.itemId = "";
  }

  private ResizeComponent(x: number,y: number): void {
    this.itemResizing.componentSettings = {
      width: this.AdjustComponentSize(Math.round(x / 10) * 10),
      height: this.AdjustComponentSize(Math.round(y / 10) * 10)
    } as ComponentSettingsModel;

    this.renderer.setStyle(this.el.nativeElement, 'height', this.itemResizing.componentSettings.height + 'px');
    this.renderer.setStyle(this.el.nativeElement, 'flex', '0 0 '+ this.itemResizing.componentSettings.width  + 'px');
  }

  private HideCursor(): void {
    let body = document.body;
    this.renderer.setStyle(body, 'user-select', 'none');
    this.renderer.setStyle(body, '-webkit-user-select', 'none');
    this.renderer.setStyle(body, '-moz-user-select', 'none');
    this.renderer.setStyle(body, '-ms-user-select', 'none');
    this.renderer.setStyle(body, 'cursor', 'none');
  }

  private ShowCursor(): void {
    let body = document.body;
    this.renderer.setStyle(body, 'user-select', 'auto');
    this.renderer.setStyle(body, '-webkit-user-select', 'auto');
    this.renderer.setStyle(body, '-moz-user-select', 'auto');
    this.renderer.setStyle(body, '-ms-user-select', 'auto');
    this.renderer.setStyle(body, 'cursor', 'auto');
  }

  private AdjustComponentSize(value: number): number {
    switch(true){
      case value > 200 && value <= 400:
        return 300;
      case value > 400 && value <= 600:
        return 500;
      case value > 600 && value <= 800:
        return 700;
      case value > 800 && value <= 800:
        return 800;
      case value > 800:
        return 800;
      default:
        return 250;
    }
  }
}
