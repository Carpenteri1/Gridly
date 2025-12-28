import {Directive, ElementRef, HostListener, Input, Renderer2} from '@angular/core';
import {ComponentService} from "../Services/component.service";
import {MapComponentData} from "../Utils/componentModel.factory";

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective {
  @Input() canResize!: boolean;
  @Input() targetId!: number;

  private isResizing: boolean = false;
  private startX: number = 0;
  private startY: number = 0;
  private startWidth: number = 0;
  private startHeight: number = 0;
  private gridItemElement: HTMLElement | null = null;
  private pointerId: number | null = null;

  constructor(private el: ElementRef,
              private renderer: Renderer2,
              private componentService: ComponentService) {}

  @HostListener('pointerdown', ['$event'])
  OnPointerDown(event: PointerEvent): void {
    if(!this.canResize || !this.targetId) return;
    
    event.preventDefault();
    event.stopPropagation();
    
    const component = this.componentService.GetComponentById(this.targetId);
    if(!component) return;
    
    this.componentService.Component = component;
    
    this.gridItemElement = this.el.nativeElement.closest('.grid-item-style') as HTMLElement;
    
    if(!this.gridItemElement) {
      let parent = this.el.nativeElement.parentElement;
      let depth = 0;
      while(parent && depth < 10) {
        if(parent.classList && parent.classList.contains('grid-item-style')) {
          this.gridItemElement = parent as HTMLElement;
          break;
        }
        parent = parent.parentElement;
        depth++;
      }
    }
    
    if(!this.gridItemElement) {
      console.warn('Could not find grid-item-style element for component', this.targetId);
      return;
    }
    
    try {
      this.el.nativeElement.setPointerCapture(event.pointerId);
      this.pointerId = event.pointerId;
    } catch (e) {
      console.warn('Failed to capture pointer:', e);
    }
    
    this.startX = event.clientX;
    this.startY = event.clientY;
    this.startWidth = component.componentSettings?.width ?? 250;
    this.startHeight = component.componentSettings?.height ?? 250;
    this.isResizing = true;
    
    this.HideCursor();
  }

  @HostListener('document:pointermove', ['$event'])
  OnPointerMove(event: PointerEvent): void {
    if(!this.isResizing || !this.gridItemElement || !this.componentService.Component) return;
    
    if(this.pointerId !== null && event.pointerId !== this.pointerId) return;
    
    const deltaX = event.clientX - this.startX;
    const deltaY = event.clientY - this.startY;
    
    const newWidth = Math.max(250, this.startWidth + deltaX);
    const newHeight = Math.max(250, this.startHeight + deltaY);
    
    const adjustedWidth = this.AdjustComponentSize(newWidth);
    const adjustedHeight = this.AdjustComponentSize(newHeight);
    
    const updatedComponent = MapComponentData.Override(
      {
        componentSettings: {
          width: adjustedWidth,
          height: adjustedHeight
        }
      },
      this.componentService.Component);
    
    this.componentService.Component = updatedComponent;
    
    if(this.componentService.Components) {
      const componentIndex = this.componentService.Components.findIndex(c => c.id === this.targetId);
      if(componentIndex !== -1) {
        this.componentService.Components[componentIndex] = updatedComponent;
      }
    }
    
    this.renderer.setStyle(this.gridItemElement, 'height', adjustedHeight + 'px');
    this.renderer.setStyle(this.gridItemElement, 'width', adjustedWidth + 'px');
    this.renderer.setStyle(this.gridItemElement, 'flex', '0 0 ' + adjustedWidth + 'px');
  }

  @HostListener('document:pointerup', ['$event'])
  OnPointerUp(event: PointerEvent): void {
    if(!this.isResizing) return;
    
    if(this.pointerId !== null) {
      try {
        this.el.nativeElement.releasePointerCapture(this.pointerId);
      } catch (e) {
      }
      this.pointerId = null;
    }
    this.ResetAll();
  }

  @HostListener('document:pointercancel', ['$event'])
  OnPointerCancel(event: PointerEvent): void {
    if(!this.isResizing) return;
    
    if(this.pointerId !== null) {
      try {
        this.el.nativeElement.releasePointerCapture(this.pointerId);
      } catch (e) {
      }
      this.pointerId = null;
    }
    this.ResetAll();
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
    this.isResizing = false;
    this.startX = 0;
    this.startY = 0;
    this.startWidth = 0;
    this.startHeight = 0;
    this.gridItemElement = null;
    this.ShowCursor();
  }

  private AdjustComponentSize(value: number): number {
    if(value < 250) return 250;
    
    switch(true){
      case value > 300 && value <= 300:
        return 300;
      case value > 400 && value <= 600:
        return 500;
      case value > 600 && value <= 800:
        return 700;
      case value > 800:
        return 800;
      default:
        return 250;
    }
  }
}
