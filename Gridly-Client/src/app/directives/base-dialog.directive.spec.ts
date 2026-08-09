import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { StubTranslatePipe } from '../testing/stub-translate.pipe';
import { DialogService } from '../services/dialog_services/dialog.service';
import { DeleteCardDialogComponent } from '../components/dialogs/deleteCardDialog/delete-card-dialog.component';

describe('BaseDialogComponent (via DeleteCardDialogComponent)', () => {
  let fixture: ComponentFixture<DeleteCardDialogComponent>;
  let component: DeleteCardDialogComponent;

  const dialogServiceMock = {
    onFileUpload: jest.fn(),
    resetImageData: jest.fn(),
  };

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    jest.clearAllMocks();

    TestBed.configureTestingModule({
      imports: [DeleteCardDialogComponent],
      providers: [
        { provide: DialogService, useValue: dialogServiceMock },
        { provide: TranslateService, useValue: translateServiceMock },
      ],
    }).overrideComponent(DeleteCardDialogComponent, {
      remove: { imports: [TranslatePipe] },
      add: { imports: [AsyncPipe, StubTranslatePipe] },
    });

    fixture = TestBed.createComponent(DeleteCardDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('delegates close() to the modal directive when present', () => {
    const closeSpy = jest.fn();
    component.modalDirective = { close: closeSpy } as never;

    component.close();

    expect(closeSpy).toHaveBeenCalled();
  });

  it('does not throw when close() is called without a modal directive', () => {
    component.modalDirective = undefined as never;

    expect(() => component.close()).not.toThrow();
  });

  it('delegates onBackdropClick() to the modal directive when present', () => {
    const backdropSpy = jest.fn();
    component.modalDirective = { onBackdropClick: backdropSpy } as never;
    const event = new MouseEvent('click');

    component.onBackdropClick(event);

    expect(backdropSpy).toHaveBeenCalledWith(event);
  });

  it('delegates onFileUpload() to the dialog service', () => {
    const event = new Event('change');
    dialogServiceMock.onFileUpload.mockResolvedValue(undefined);

    component.onFileUpload(event);

    expect(dialogServiceMock.onFileUpload).toHaveBeenCalledWith(event);
  });

  it('delegates resetImageData() to the dialog service', () => {
    component.resetImageData();

    expect(dialogServiceMock.resetImageData).toHaveBeenCalled();
  });

  it('reports isOpen based on the modal directive open state', () => {
    component.modalDirective = { open: true } as never;
    expect(component.isOpen).toBe(true);

    component.modalDirective = { open: false } as never;
    expect(component.isOpen).toBe(false);
  });

  it('reports isOpen as false when there is no modal directive', () => {
    component.modalDirective = undefined as never;
    expect(component.isOpen).toBe(false);
  });

  it('submit() closes the dialog', async () => {
    const closeSpy = jest.fn();
    component.modalDirective = { close: closeSpy } as never;

    await component.submit();

    expect(closeSpy).toHaveBeenCalled();
  });
});
