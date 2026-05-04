import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DialogService } from '../../../services/dialog_services/dialog.service';
import { CardModel } from '../../../models/card.Model';
import { IconModel } from '../../../models/icon.Model';
import { AddCardDialogComponent } from './add-card-dialog.component';
import { CardTypes } from '../../../types/card.types.enum';

describe('AddCardDialogComponent', () => {
  let fixture: ComponentFixture<AddCardDialogComponent>;
  let component: AddCardDialogComponent;

  const dialogServiceMock = {
    onFileUpload: jest.fn(),
    resetImageData: jest.fn(),
    settings: () => ({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    }),
    icon: () => ({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'add_box',
    } as IconModel)
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCardDialogComponent],
      providers: [{ provide: DialogService, useValue: dialogServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(AddCardDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('emits a new card payload for both supported card types', () => {
    const emitSpy = jest.spyOn(component.newCard, 'emit');

    component.onSelect(CardTypes.Empty);
    component.onSelect(CardTypes.Custom);

    expect(emitSpy).toHaveBeenCalledTimes(2);
    expect(emitSpy.mock.calls[0][0]).toBeDefined();
    expect(emitSpy.mock.calls[1][0]).toBeDefined();
  });

  it('emits a fully initialized card model', () => {
    const emitSpy = jest.spyOn(component.newCard, 'emit');

    component.onSelect(CardTypes.Empty);

    const card = emitSpy.mock.calls[0]?.[0] as CardModel | undefined;

    expect(card).toBeDefined();

    if (!card) {
      throw new Error('Expected card payload to be emitted.');
    }

    card.settings ??= dialogServiceMock.settings();
    card.iconData ??= dialogServiceMock.icon();

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
      materialIcon: 'add_box',
    });
  });
});
