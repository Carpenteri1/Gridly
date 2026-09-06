import { ChangeDetectionStrategy, Component, ViewChild } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { CardModel } from '../models/card.Model';
import { ResizableDirective } from './resizable.directive';

@Component({
  standalone: true,
  imports: [ResizableDirective],
  // Angular 22 defaults components without an explicit strategy to OnPush; this test host
  // reassigns @Input()-bound fields directly and relies on plain (non-OnPush) re-checks.
  // eslint-disable-next-line @angular-eslint/prefer-on-push-component-change-detection
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    <div class="grid-card-style">
      <div class="resize-handle" appMakeResizable [canResize]="canResize" [targetCard]="targetCard"></div>
    </div>
  `,
})
class TestHostComponent {
  @ViewChild(ResizableDirective) directive!: ResizableDirective;
  canResize = true;
  targetCard: CardModel = Object.assign(new CardModel(), {
    id: 1,
    indexPosition: 1,
    name: 'Alpha',
    url: 'https://alpha.example',
  });
}

// jsdom (as used by jest-environment-jsdom) does not implement PointerEvent,
// so a MouseEvent is used as a stand-in and augmented with a pointerId.
function pointerEvent(type: string, init: { pointerId: number; clientX?: number; clientY?: number }): Event {
  const event = new MouseEvent(type, {
    bubbles: true,
    clientX: init.clientX ?? 0,
    clientY: init.clientY ?? 0,
  });
  Object.defineProperty(event, 'pointerId', { value: init.pointerId, configurable: true });
  return event;
}

describe('ResizableDirective', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let host: TestHostComponent;
  let handle: HTMLElement;
  let cardEl: HTMLElement;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [TestHostComponent] });

    fixture = TestBed.createComponent(TestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();

    handle = fixture.debugElement.query(By.css('.resize-handle')).nativeElement;
    cardEl = fixture.debugElement.query(By.css('.grid-card-style')).nativeElement;

    // jsdom does not implement pointer capture.
    handle.setPointerCapture = jest.fn();
    handle.releasePointerCapture = jest.fn();
  });

  afterEach(() => {
    document.body.removeAttribute('style');
  });

  it('does nothing on pointerdown when canResize is false', () => {
    host.canResize = false;
    fixture.detectChanges();

    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    expect(handle.setPointerCapture).not.toHaveBeenCalled();
  });

  it('captures the pointer and hides the cursor on pointerdown', () => {
    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    expect(handle.setPointerCapture).toHaveBeenCalledWith(1);
    expect(document.body.style.cursor).toBe('none');
  });

  it('falls back to walking up parentElement when closest() cannot find the card', () => {
    handle.closest = jest.fn(() => null);

    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    expect(handle.setPointerCapture).toHaveBeenCalledWith(1);
  });

  it('does nothing on pointerdown when no .grid-card-style ancestor exists', () => {
    cardEl.classList.remove('grid-card-style');

    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    expect(handle.setPointerCapture).not.toHaveBeenCalled();
  });

  it('ignores pointermove events when no drag has started', () => {
    document.dispatchEvent(pointerEvent('pointermove', { pointerId: 1, clientX: 500, clientY: 500 }));

    expect(cardEl.style.width).toBe('');
  });

  it('ignores pointermove events from a different pointer than the one that started the drag', () => {
    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    document.dispatchEvent(pointerEvent('pointermove', { pointerId: 2, clientX: 500, clientY: 500 }));

    expect(cardEl.style.width).toBe('');
  });

  it.each([
    [0, 250],
    [150, 300],
    [350, 500],
    [550, 700],
    [700, 800],
  ])('snaps the card size to the nearest breakpoint (deltaX=%d -> %dpx)', (deltaX, expectedSize) => {
    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));
    document.dispatchEvent(pointerEvent('pointermove', { pointerId: 1, clientX: deltaX, clientY: 0 }));

    expect(host.targetCard.settings?.width).toBe(expectedSize);
    expect(host.targetCard.settings?.height).toBe(250);
    expect(cardEl.style.width).toBe(`${expectedSize}px`);
    expect(cardEl.style.flex).toBe(`0 0 ${expectedSize}px`);
  });

  it('releases the pointer and restores the cursor on pointerup', () => {
    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    document.dispatchEvent(pointerEvent('pointerup', { pointerId: 1 }));

    expect(handle.releasePointerCapture).toHaveBeenCalledWith(1);
    expect(document.body.style.cursor).toBe('auto');
  });

  it('releases the pointer and restores the cursor on pointercancel', () => {
    handle.dispatchEvent(pointerEvent('pointerdown', { pointerId: 1, clientX: 0, clientY: 0 }));

    document.dispatchEvent(pointerEvent('pointercancel', { pointerId: 1 }));

    expect(handle.releasePointerCapture).toHaveBeenCalledWith(1);
    expect(document.body.style.cursor).toBe('auto');
  });

  it('ignores pointerup when no drag is in progress', () => {
    document.dispatchEvent(pointerEvent('pointerup', { pointerId: 1 }));

    expect(handle.releasePointerCapture).not.toHaveBeenCalled();
  });
});
