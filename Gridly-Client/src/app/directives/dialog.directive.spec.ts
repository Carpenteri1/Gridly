import { ChangeDetectionStrategy, Component, ViewChild } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DialogDirective } from './dialog.directive';

@Component({
  standalone: true,
  imports: [DialogDirective],
  // Angular 22 defaults components without an explicit strategy to OnPush; this test host
  // reassigns @Input()-bound fields directly and relies on plain (non-OnPush) re-checks.
  // eslint-disable-next-line @angular-eslint/prefer-on-push-component-change-detection
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    <dialog appDialogWindow [dialogId]="dialogId" [id]="id" [open]="open" (openChange)="onOpenChange($event)">
      <button type="button" class="inner-btn">inner</button>
    </dialog>
  `,
})
class TestHostComponent {
  @ViewChild(DialogDirective) directive!: DialogDirective;
  dialogId = 1;
  id = 1;
  open = false;
  lastEmitted: number | undefined;

  onOpenChange(id: number): void {
    this.lastEmitted = id;
  }
}

describe('DialogDirective', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let host: TestHostComponent;
  let dialogEl: HTMLDialogElement;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TestHostComponent],
    });

    fixture = TestBed.createComponent(TestHostComponent);
    host = fixture.componentInstance;
    dialogEl = fixture.debugElement.query(By.css('dialog')).nativeElement;

    // jsdom does not implement HTMLDialogElement.showModal/close.
    dialogEl.showModal = jest.fn(() => {
      Object.defineProperty(dialogEl, 'open', { value: true, configurable: true });
    });
    dialogEl.close = jest.fn(() => {
      Object.defineProperty(dialogEl, 'open', { value: false, configurable: true });
    });

    fixture.detectChanges();
  });

  it('opens the native dialog when open becomes true and the ids match', () => {
    host.open = true;
    fixture.detectChanges();

    expect(dialogEl.showModal).toHaveBeenCalledTimes(1);
  });

  it('does not open the dialog when the dialogId does not match id', () => {
    host.dialogId = 2;
    host.open = true;
    fixture.detectChanges();

    expect(dialogEl.showModal).not.toHaveBeenCalled();
  });

  it('closes the native dialog when open becomes false while it is open', () => {
    host.open = true;
    fixture.detectChanges();
    (dialogEl.showModal as jest.Mock).mockClear();

    host.open = false;
    fixture.detectChanges();

    expect(dialogEl.close).toHaveBeenCalledTimes(1);
  });

  it('emits openChange with the dialogId when the native close event fires', () => {
    dialogEl.dispatchEvent(new Event('close'));

    expect(host.lastEmitted).toBe(host.dialogId);
  });

  it('emits openChange with the dialogId when the native cancel event fires', () => {
    dialogEl.dispatchEvent(new Event('cancel'));

    expect(host.lastEmitted).toBe(host.dialogId);
  });

  it('close() calls the native close and emits openChange', () => {
    host.directive.close();

    expect(dialogEl.close).toHaveBeenCalled();
    expect(host.lastEmitted).toBe(host.dialogId);
  });

  it('save() delegates to close()', () => {
    const closeSpy = jest.spyOn(host.directive, 'close');

    host.directive.save();

    expect(closeSpy).toHaveBeenCalled();
  });

  it('closes when the backdrop itself is clicked', () => {
    dialogEl.dispatchEvent(new MouseEvent('click', { bubbles: true }));

    expect(dialogEl.close).toHaveBeenCalled();
  });

  it('does not close when an inner element is clicked', () => {
    const innerButton = fixture.debugElement.query(By.css('.inner-btn')).nativeElement as HTMLElement;

    innerButton.dispatchEvent(new MouseEvent('click', { bubbles: true }));

    expect(dialogEl.close).not.toHaveBeenCalled();
  });
});
