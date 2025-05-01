import {Directive, ElementRef, HostListener, Renderer2, OnInit, Input} from '@angular/core';

@Directive({
  standalone: true,
  selector: '[makeResizable]'
})

export class ResizableDirective implements OnInit {
  private isResizing!: boolean;
  @Input() itemResizing!: any;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit() {
    this.isResizing = false;
  }

  @HostListener('pointerdown', ['$event'])
  OnLeftMouseClickPress(): void {
    this.isResizing = true;
  }

  @HostListener('pointerup', ['$event'])
  OnLeftMouseClickRelease(): void {
    this.isResizing = false;
  }

  @HostListener('document:pointermove', ['$event'])
  OnMouseMoveDown(event: MouseEvent): void {
    setTimeout(() => {
    if(this.isResizing){
        this.ResizeComponent(event.offsetX, event.offsetY);
      }
    },400);
  }

  private ResizeComponent(x: number,y: number): void {
    let snappedWidth = Math.round(x / 10) * 10;
    let snappedHeight = Math.round(y / 10) * 10;

    if (snappedWidth < 200 && snappedHeight < 250){
      snappedWidth = 160;
      snappedHeight = 200;
    }

    this.renderer.setStyle(this.el.nativeElement, 'flex', '0 0' + snappedWidth + 'px');
    this.renderer.setStyle(this.el.nativeElement, 'height', snappedHeight + 'px');
  }
}
