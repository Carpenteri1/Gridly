import {Directive, ElementRef, HostListener, Renderer2, OnInit, Input} from '@angular/core';
import {ComponentSettingsModel} from "../Models/ComponentSettings.Model";

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective implements OnInit {
  private isResizing!: boolean;
  @Input() itemResizing!: any;
  @Input() canResize!: boolean;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit() {
    this.isResizing = false;
  }

  @HostListener('document:pointermove', ['$event'])
  OnMouseMove(event: MouseEvent): void {
    if(!this.canResize) return;

    if(!this.isResizing){
      this.isResizing = true;
      this.ResizeComponent(event.offsetX, event.offsetY);
      setTimeout(() => {
        this.isResizing = false;
      },200);
    }
  }

  @HostListener('document:pointerdown', ['$event'])
  OnLeftMouseButtonPress(){
    this.HideCursor();
  }

  @HostListener('document:pointerup', ['$event'])
  OnLeftMouseButtonRelease(){
    this.ShowCursor();
  }

  private ResizeComponent(x: number,y: number): void {
    this.itemResizing.componentSettings = new ComponentSettingsModel(
      this.AdjustComponentSize(Math.round(x / 10) * 10),
      this.AdjustComponentSize(Math.round(y / 10) * 10));

    this.renderer.setStyle(this.el.nativeElement, 'height', this.itemResizing.componentSettings.height + 'px');
    this.renderer.setStyle(this.el.nativeElement, 'flex', '0 0 '+ this.itemResizing.componentSettings.width  + 'px');
  }

  private HideCursor(): void {
      this.renderer.setStyle(document.body, 'user-select', 'none');
      this.renderer.setStyle(document.body, '-webkit-user-select', 'none');
      this.renderer.setStyle(document.body, '-moz-user-select', 'none');
      this.renderer.setStyle(document.body, '-ms-user-select', 'none');
      this.renderer.setStyle(document.body, 'cursor', 'none');
  }

  private ShowCursor(): void {
    this.renderer.setStyle(document.body, 'user-select', 'auto');
    this.renderer.setStyle(document.body, '-webkit-user-select', 'auto');
    this.renderer.setStyle(document.body, '-moz-user-select', 'auto');
    this.renderer.setStyle(document.body, '-ms-user-select', 'auto');
    this.renderer.setStyle(document.body, 'cursor', 'auto');
  }

  private AdjustComponentSize(value: number): number {
    if (value <= 200) return 200;
    if (value <= 400) return 300;
    if (value <= 600) return 500;
    if (value <= 800) return 700;
    return 800;
  }
}
