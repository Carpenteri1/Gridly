import { Directive, ElementRef, HostListener, inject, Input, Renderer2 } from '@angular/core';
import { CardService } from "../services/card.service";

@Directive({
  standalone: true,
  selector: '[appMakeResizable]'
})

export class ResizableDirective {
  private el = inject(ElementRef);
  private renderer = inject(Renderer2);

  @Input() canResize!: boolean;
  @Input() targetId!: number;

  #cardService = inject(CardService);
  
  private cards = this.#cardService.currentcard();

  private isResizing = false;
  private startX = 0;
  private startY = 0;
  private startWidth = 0;
  private startHeight = 0;
  private gridItemElement: HTMLElement | null = null;
  private pointerId: number | null = null;

  @HostListener('pointerdown', ['$event'])
  OnPointerDown(event: PointerEvent): void {
    if (!this.canResize || !this.targetId) return;

    event.preventDefault();
    event.stopPropagation();

    const card = this.cards?.find((currentcard) => currentcard.id === this.targetId);
    if (!card) return;

    this.gridItemElement = this.el.nativeElement.closest('.grid-item-style') as HTMLElement;

    if (!this.gridItemElement) {
      let parent = this.el.nativeElement.parentElement;
      let depth = 0;
      while (parent && depth < 10) {
        if (parent.classList && parent.classList.contains('grid-item-style')) {
          this.gridItemElement = parent as HTMLElement;
          break;
        }
        parent = parent.parentElement;
        depth++;
      }
    }
    
    if (!this.gridItemElement) {
      console.warn('Could not find grid-item-style element for card', this.targetId);
      return;
    }

    try {
      this.el.nativeElement.setPointerCapture(event.pointerId);
      this.pointerId = event.pointerId;
    } catch {
      console.warn('Failed to capture pointer.');
    }

    this.startX = event.clientX;
    this.startY = event.clientY;
    this.startWidth = card.settings?.width ?? 250;
    this.startHeight = card.settings?.height ?? 250;
    this.isResizing = true;

    this.HideCursor();
  }

  @HostListener('document:pointermove', ['$event'])
  OnPointerMove(event: PointerEvent): void {
    const card = this.cards?.find((currentcard) => currentcard.id === this.targetId);
    if (!this.isResizing || !this.gridItemElement || !card) return;

    if (this.pointerId !== null && event.pointerId !== this.pointerId) return;

    const deltaX = event.clientX - this.startX;
    const deltaY = event.clientY - this.startY;

    const newWidth = Math.max(250, this.startWidth + deltaX);
    const newHeight = Math.max(250, this.startHeight + deltaY);

    const adjustedWidth = this.Adjustcardsize(newWidth);
    const adjustedHeight = this.Adjustcardsize(newHeight);
    
    card.settings = {
      ...card.settings,
      width: adjustedWidth,
      height: adjustedHeight
    };
    
    this.renderer.setStyle(this.gridItemElement, 'height', adjustedHeight + 'px');
    this.renderer.setStyle(this.gridItemElement, 'width', adjustedWidth + 'px');
    this.renderer.setStyle(this.gridItemElement, 'flex', '0 0 ' + adjustedWidth + 'px');
  }

  @HostListener('document:pointerup')
  OnPointerUp(): void {
    if (!this.isResizing) return;

    if (this.pointerId !== null) {
      try {
        this.el.nativeElement.releasePointerCapture(this.pointerId);
      } catch {
        // Ignore release errors when capture is already lost.
      }
      this.pointerId = null;
    }
    this.ResetAll();
  }

  @HostListener('document:pointercancel')
  OnPointerCancel(): void {
    if (!this.isResizing) return;

    if (this.pointerId !== null) {
      try {
        this.el.nativeElement.releasePointerCapture(this.pointerId);
      } catch {
        // Ignore release errors when capture is already lost.
      }
      this.pointerId = null;
    }
    this.ResetAll();
  }

  private HideCursor(): void {
    const body = document.body;
    this.renderer.setStyle(body, 'user-select', 'none');
    this.renderer.setStyle(body, '-webkit-user-select', 'none');
    this.renderer.setStyle(body, '-moz-user-select', 'none');
    this.renderer.setStyle(body, '-ms-user-select', 'none');
    this.renderer.setStyle(body, 'cursor', 'none');
  }

  private ShowCursor(): void {
    const body = document.body;
    this.renderer.setStyle(body, 'user-select', 'auto');
    this.renderer.setStyle(body, '-webkit-user-select', 'auto');
    this.renderer.setStyle(body, '-moz-user-select', 'auto');
    this.renderer.setStyle(body, '-ms-user-select', 'auto');
    this.renderer.setStyle(body, 'cursor', 'auto');
  }

  private ResetAll(): void {
    this.isResizing = false;
    this.startX = 0;
    this.startY = 0;
    this.startWidth = 0;
    this.startHeight = 0;
    this.gridItemElement = null;
    this.ShowCursor();
  }

  private Adjustcardsize(value: number): number {
    if (value <= 300) return 250;
    if (value <= 500) return 300;
    if (value <= 700) return 500;
    if (value <= 800) return 700;
    return 800;
  }
}
