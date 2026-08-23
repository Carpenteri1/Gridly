import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { StubTranslatePipe } from '../../../testing/stub-translate.pipe';
import { PromptDialogComponent } from './prompt.dialog.component';

describe('PromptDialogComponent', () => {
  let fixture: ComponentFixture<PromptDialogComponent>;
  let component: PromptDialogComponent;

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [PromptDialogComponent],
      providers: [{ provide: TranslateService, useValue: translateServiceMock }],
    }).overrideComponent(PromptDialogComponent, {
      remove: { imports: [TranslatePipe] },
      add: { imports: [AsyncPipe, StubTranslatePipe] },
    });

    fixture = TestBed.createComponent(PromptDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('closes and emits the card id to remove on submit', () => {
    jest.spyOn(component, 'close').mockImplementation(() => undefined);
    const removeSpy = jest.spyOn(component.remove, 'emit');
    component.id = 7;

    component.onSubmit();

    expect(component.close).toHaveBeenCalled();
    expect(removeSpy).toHaveBeenCalledWith({ id: 7 });
  });
});
