import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { DialogService } from '../../../services/dialog_services/dialog.service';
import { CardModel } from '../../../models/card.Model';
import { IconModel } from '../../../models/icon.Model';
import { AddCardDialogComponent } from './add-card-dialog.component';
import { CardTypes } from '../../../enums/card.types.enum';
import { Widget } from '../../../interfaces/widget.Interface';

describe('AddCardDialogComponent', () => {
  let fixture: ComponentFixture<AddCardDialogComponent>;
  let dialogComponent: AddCardDialogComponent;

  const dialogServiceMock = {
    onFileUpload: jest.fn(),
    resetImageData: jest.fn(),
    settings: () => ({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    }),
    setIcon: () => ({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'box',
    } as IconModel)
  };

  const widget = (widgetType: CardTypes): Widget => ({
    id: 1,
    widgetType,
    label: '',
    description: '',
    icon: '',
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AddCardDialogComponent],
      providers: [{ provide: DialogService, useValue: dialogServiceMock }],
    }).overrideComponent(AddCardDialogComponent, {
      add: { imports: [AsyncPipe] },
    });

    fixture = TestBed.createComponent(AddCardDialogComponent);
    dialogComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('emits a new card payload for both supported card types', () => {
    const emitSpy = jest.spyOn(dialogComponent.newCard, 'emit');

    dialogComponent.onSelect(widget(CardTypes.Empty));
    dialogComponent.onSelect(widget(CardTypes.Custom));

    expect(emitSpy).toHaveBeenCalledTimes(2);
    expect(emitSpy.mock.calls[0][0]).toBeDefined();
    expect(emitSpy.mock.calls[1][0]).toBeDefined();
  });

  it('emits a fully initialized card model', () => {
    const emitSpy = jest.spyOn(dialogComponent.newCard, 'emit');

    dialogComponent.onSelect(widget(CardTypes.Empty));

    const card = emitSpy.mock.calls[0]?.[0] as CardModel | undefined;

    expect(card).toBeDefined();

    if (!card) {
      throw new Error('Expected card payload to be emitted.');
    }

    card.settings ??= dialogServiceMock.settings();
    card.iconData ??= dialogServiceMock.setIcon();

    expect(card.settings).toEqual({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    });
    expect(card.iconData).toEqual({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'box',
    });
  });

  it('shows the timezone/format picker instead of emitting immediately for Clock', () => {
    const emitSpy = jest.spyOn(dialogComponent.newCard, 'emit');

    dialogComponent.onSelect(widget(CardTypes.Clock));

    expect(emitSpy).not.toHaveBeenCalled();
  });

  it('emits a Clock card with the selected timezone and display format on confirm', () => {
    const emitSpy = jest.spyOn(dialogComponent.newCard, 'emit');

    dialogComponent.onSelect(widget(CardTypes.Clock));
    (dialogComponent as unknown as { selectedTimeZone: { set(v: string): void } })
      .selectedTimeZone.set('Europe/Stockholm');
    (dialogComponent as unknown as { selectedDisplayFormat: { set(v: string): void } })
      .selectedDisplayFormat.set('analog');
    dialogComponent.confirmClock();

    expect(emitSpy).toHaveBeenCalledTimes(1);
    const card = emitSpy.mock.calls[0][0] as CardModel;
    expect(card.type).toBe(CardTypes.Clock);
    expect(card.settings?.timeZone).toBe('Europe/Stockholm');
    expect(card.settings?.displayFormat).toBe('analog');
  });

  it('discards the picker without emitting when the Clock config is cancelled', () => {
    const emitSpy = jest.spyOn(dialogComponent.newCard, 'emit');

    dialogComponent.onSelect(widget(CardTypes.Clock));
    dialogComponent.cancelClockConfig();
    dialogComponent.confirmClock();

    expect(emitSpy).not.toHaveBeenCalled();
  });
});
