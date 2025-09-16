import {Directive, ElementRef, HostListener, Input, Renderer2} from '@angular/core';
import {ComponentModel} from "../Models/Component.Model";
import {ComponentService} from "../Services/component.service";
import {MapComponentData} from "../Utils/componentModal.factory";

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective {
  @Input() canResize!: boolean;
  targetId!: number;

  constructor(private el: ElementRef,
              private renderer: Renderer2,
              private componentService: ComponentService) {}

  @HostListener('document:pointermove', ['$event'])
  async OnMouseMove(event: MouseEvent): Promise<void> {
    if(!this.canResize) return;
    const target = event.target as HTMLElement;
    if(target.id !== "") {
      if(this.componentService.Component === undefined ||
        this.componentService.Component.id !== this.targetId)
      {
        this.componentService.Component = this.componentService.GetComponentById(this.targetId)!;
      }
      this.ResizeComponent(event.clientX, event.clientY);
    }
  }

  @HostListener('pointerdown', ['$event'])
  OnLeftMouseButtonPress(event: MouseEvent): void {
    this.HideCursor();
    const target = event.target as HTMLElement;
    if(target.id === "")
    {
      this.targetId = Number(target.id);
      this.componentService.Component = this.componentService.GetComponentById(this.targetId) as ComponentModel;
    }
  }

  @HostListener('document:pointerup', ['$event'])
  OnLeftMouseButtonRelease(){
    this.ResetAll();
  }

  private ResizeComponent(x: number,y: number): void {
    this.componentService.Component = MapComponentData.Override(
      {
        componentSettings: {
          width: this.AdjustComponentSize(Math.round(x / 10) * 10),
          height: this.AdjustComponentSize(Math.round(y / 10) * 10)}
      },
      this.componentService.Component);

    this.renderer.setStyle(this.el.nativeElement, 'height', this.componentService.Component.componentSettings!.height + 'px');
    this.renderer.setStyle(this.el.nativeElement, 'flex', '0 0 ' + this.componentService.Component.componentSettings!.width  + 'px');
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

  private ResetAll(){
    this.ShowCursor();
    this.targetId = 0;
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
