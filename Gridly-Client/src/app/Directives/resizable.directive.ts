import {Directive, ElementRef, HostListener, Renderer2, OnInit, Input} from '@angular/core';

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
    if(this.canResize && !this.isResizing){
      this.isResizing = true;
      this.ResizeComponent(event.offsetX, event.offsetY);
      setTimeout(() => {
        this.isResizing = false;
      },200);
    }
  }

  private ResizeComponent(x: number,y: number): void {
    let snappedWidth = this.AdjustComponentSize(Math.round(x / 10) * 10);
    let snappedHeight = this.AdjustComponentSize(Math.round(y / 10) * 10);

    this.renderer.setStyle(this.el.nativeElement, 'height', snappedHeight + 'px');
    this.renderer.setStyle(this.el.nativeElement, 'flex', '0 0 '+ snappedWidth + 'px');
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
        return 200;
    }
  }
}
