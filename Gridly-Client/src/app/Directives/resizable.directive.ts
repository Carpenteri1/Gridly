import {Directive, ElementRef, HostListener, Input, Renderer2} from '@angular/core';
import {ComponentModel} from "../Models/Component.Model";
import {ComponentService} from "../Services/component.service";
import {MapComponentData} from "../Utils/componentDialog.factory";

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
    
    // Get the component first
    const component = this.componentService.GetComponentById(this.targetId);
    if(!component) return;
    
    this.componentService.Component = component;
    
    // Find the parent grid item element
    // The button is inside item-component, which is inside grid-item-style
    // Try multiple approaches to find the grid item
    this.gridItemElement = this.el.nativeElement.closest('.grid-item-style') as HTMLElement;
    
    if(!this.gridItemElement) {
      // Fallback: traverse up the DOM tree
      let parent = this.el.nativeElement.parentElement;
      let depth = 0;
      while(parent && depth < 10) { // Limit depth to prevent infinite loops
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
    
    // Capture pointer to keep focus on button
    try {
      this.el.nativeElement.setPointerCapture(event.pointerId);
      this.pointerId = event.pointerId;
    } catch (e) {
      console.warn('Failed to capture pointer:', e);
    }
    
    // Store initial state
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
    
    // Only process if this is our captured pointer (if we have one)
    if(this.pointerId !== null && event.pointerId !== this.pointerId) return;
    
    // Calculate deltas from initial position
    const deltaX = event.clientX - this.startX;
    const deltaY = event.clientY - this.startY;
    
    // Calculate new dimensions (right/down = bigger, left/up = smaller)
    const newWidth = Math.max(250, this.startWidth + deltaX);
    const newHeight = Math.max(250, this.startHeight + deltaY);
    
    // Apply size constraints
    const adjustedWidth = this.AdjustComponentSize(newWidth);
    const adjustedHeight = this.AdjustComponentSize(newHeight);
    
    // Update component model
    const updatedComponent = MapComponentData.Override(
      {
        componentSettings: {
          width: adjustedWidth,
          height: adjustedHeight
        }
      },
      this.componentService.Component);
    
    this.componentService.Component = updatedComponent;
    
    // Update component in the Components array to persist changes
    if(this.componentService.Components) {
      const componentIndex = this.componentService.Components.findIndex(c => c.id === this.targetId);
      if(componentIndex !== -1) {
        this.componentService.Components[componentIndex] = updatedComponent;
      }
    }
    
    // Apply styles to grid item element (matching SetLayout pattern)
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
        // Ignore errors if pointer was already released
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
        // Ignore errors
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
    // Ensure minimum size
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
