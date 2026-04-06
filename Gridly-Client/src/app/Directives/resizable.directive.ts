import {Directive, ElementRef, HostListener, inject, Input, Renderer2} from '@angular/core';
import {ComponentService} from "../Services/component.service";
import { ComponentModel } from '../Models/Component.Model';

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective {
  @Input() canResize!: boolean;
  @Input() targetId!: number;

  #componentService = inject(ComponentService);
  
  private component = this.#componentService.currentComponent() as ComponentModel;
  private components = this.#componentService.currentComponents() as ComponentModel[];

  private isResizing: boolean = false;
  private startX: number = 0;
  private startY: number = 0;
  private startWidth: number = 0;
  private startHeight: number = 0;
  private gridItemElement: HTMLElement | null = null;
  private pointerId: number | null = null;

  constructor(private el: ElementRef,
              private renderer: Renderer2) {}

  @HostListener('pointerdown', ['$event'])
  OnPointerDown(event: PointerEvent): void {
    if(!this.canResize || !this.targetId) return;
    
    event.preventDefault();
    event.stopPropagation();
    

    if(!this.component) return;
    
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
    this.startWidth = this.component.componentSettings?.width ?? 250;
    this.startHeight = this.component . componentSettings?.height ?? 250;
    this.isResizing = true;
    
    this.HideCursor();
  }

  @HostListener('document:pointermove', ['$event'])
  OnPointerMove(event: PointerEvent): void {
    if(!this.isResizing || !this.gridItemElement || !this.component) return;
    
    if(this.pointerId !== null && event.pointerId !== this.pointerId) return;
    
    const deltaX = event.clientX - this.startX;
    const deltaY = event.clientY - this.startY;
    
    const newWidth = Math.max(250, this.startWidth + deltaX);
    const newHeight = Math.max(250, this.startHeight + deltaY);
    
    const adjustedWidth = this.AdjustComponentSize(newWidth);
    const adjustedHeight = this.AdjustComponentSize(newHeight);
    
    const updatedComponent = this.component;
    updatedComponent.componentSettings = {
      ...updatedComponent.componentSettings,
      width: adjustedWidth,
      height: adjustedHeight
    };

    this.component = updatedComponent;
    
    if(this.components) {
      const componentIndex = this.components.findIndex(c => c.id === this.targetId);
      if(componentIndex !== -1) {
        this.components[componentIndex] = updatedComponent;
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
function take(arg0: number): import("rxjs").OperatorFunction<import("../Models/Component.Model").ComponentModel, unknown> {
  throw new Error('Function not implemented.');
}

