import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { StubTranslatePipe } from '../../../testing/stub-translate.pipe';
import { DeleteCardDialogComponent } from './delete-card-dialog.component';

describe('DeleteCardDialogComponent', () => {
  let fixture: ComponentFixture<DeleteCardDialogComponent>;
  let component: DeleteCardDialogComponent;

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [DeleteCardDialogComponent],
      providers: [{ provide: TranslateService, useValue: translateServiceMock }],
    }).overrideComponent(DeleteCardDialogComponent, {
      remove: { imports: [TranslatePipe] },
      add: { imports: [AsyncPipe, StubTranslatePipe] },
    });

    fixture = TestBed.createComponent(DeleteCardDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('closes and emits the card id to remove on submit', () => {
    jest.spyOn(component, 'close').mockImplementation(() => undefined);
    const removeSpy = jest.spyOn(component.remove, 'emit');
    component.id = 42;

    component.onSubmit();

    expect(component.close).toHaveBeenCalled();
    expect(removeSpy).toHaveBeenCalledWith({ id: 42 });
  });
});
