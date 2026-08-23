import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { of } from 'rxjs';
import { StubTranslatePipe } from '../../../testing/stub-translate.pipe';
import { CardModel } from '../../../models/card.Model';
import { EditCardDialogFacade } from './edit-card-dialog.facade';
import { EditCardDialogComponent } from './edit-card-dialog.component';

describe('EditCardDialogComponent', () => {
  let fixture: ComponentFixture<EditCardDialogComponent>;
  let component: EditCardDialogComponent;

  const facadeMock = {
    card: new CardModel(),
    icons$: of(null),
    countryInput: '',
    cityInput: '',
    errorStatus: 0,
    isWeatherCard: false,
    onSearch: jest.fn(),
    setIcon: jest.fn(),
    saveLocation: jest.fn(),
    buildSubmitPayload: jest.fn(),
    reset: jest.fn(),
  };

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    jest.clearAllMocks();
    facadeMock.isWeatherCard = false;
    facadeMock.card = new CardModel();

    TestBed.configureTestingModule({
      imports: [EditCardDialogComponent],
      providers: [{ provide: TranslateService, useValue: translateServiceMock }],
    })
      .overrideComponent(EditCardDialogComponent, {
        remove: { imports: [TranslatePipe] },
        add: { imports: [AsyncPipe, StubTranslatePipe] },
      })
      .overrideComponent(EditCardDialogComponent, {
        set: { providers: [{ provide: EditCardDialogFacade, useValue: facadeMock }] },
      });

    fixture = TestBed.createComponent(EditCardDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    jest.spyOn(component, 'close').mockImplementation(() => undefined);
  });

  it('resets the facade with the current card when the dialog opens', () => {
    const card = new CardModel();
    component.card = card;

    component.ngOnChanges({
      open: { currentValue: true, previousValue: false, firstChange: false, isFirstChange: () => false },
    });

    expect(facadeMock.reset).toHaveBeenCalledWith(card);
  });

  it('resets the facade with undefined when the dialog opens without a card', () => {
    component.card = undefined;

    component.ngOnChanges({
      open: { currentValue: true, previousValue: false, firstChange: false, isFirstChange: () => false },
    });

    expect(facadeMock.reset).toHaveBeenCalledWith(undefined);
  });

  it('does not reset the facade when a change other than opening occurs', () => {
    component.ngOnChanges({
      open: { currentValue: false, previousValue: true, firstChange: false, isFirstChange: () => false },
    });

    expect(facadeMock.reset).not.toHaveBeenCalled();
  });

  it('saves the location first for weather cards and stops if it fails', async () => {
    facadeMock.isWeatherCard = true;
    facadeMock.saveLocation.mockResolvedValue(false);
    const editCardSpy = jest.spyOn(component.editCard, 'emit');
    component.id = 5;

    await component.onSubmit();

    expect(facadeMock.saveLocation).toHaveBeenCalledWith(5);
    expect(facadeMock.buildSubmitPayload).not.toHaveBeenCalled();
    expect(component.close).not.toHaveBeenCalled();
    expect(editCardSpy).not.toHaveBeenCalled();
  });

  it('builds and emits the payload for weather cards when the location save succeeds', async () => {
    facadeMock.isWeatherCard = true;
    facadeMock.saveLocation.mockResolvedValue(true);
    const payload = new CardModel();
    facadeMock.buildSubmitPayload.mockReturnValue(payload);
    const editCardSpy = jest.spyOn(component.editCard, 'emit');
    component.id = 5;

    await component.onSubmit();

    expect(facadeMock.buildSubmitPayload).toHaveBeenCalledWith(5);
    expect(component.close).toHaveBeenCalled();
    expect(editCardSpy).toHaveBeenCalledWith(payload);
  });

  it('builds and emits the payload directly for non-weather cards', async () => {
    facadeMock.isWeatherCard = false;
    const payload = new CardModel();
    facadeMock.buildSubmitPayload.mockReturnValue(payload);
    const editCardSpy = jest.spyOn(component.editCard, 'emit');
    component.id = 9;

    await component.onSubmit();

    expect(facadeMock.saveLocation).not.toHaveBeenCalled();
    expect(facadeMock.buildSubmitPayload).toHaveBeenCalledWith(9);
    expect(component.close).toHaveBeenCalled();
    expect(editCardSpy).toHaveBeenCalledWith(payload);
  });
});
