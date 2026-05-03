import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ModalService } from '../../../services/modal.service';
import { CardModel } from '../../../models/card.Model';
import { IconModel } from '../../../models/icon.Model';
import { AddCardDialogComponent } from './add-card-dialog.component';
import { CardTypes } from '../../../types/card.types.enum';

describe('AddCardDialogComponent', () => {
  let fixture: ComponentFixture<AddCardDialogComponent>;
  let component: AddCardDialogComponent;

  const modalServiceMock = {
    onFileUpload: jest.fn(),
    resetImageData: jest.fn(),
    componentSettings: () => ({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    }),
    iconSettings: () => ({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'add_box',
    } as IconModel),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCardDialogComponent],
      providers: [{ provide: ModalService, useValue: modalServiceMock }],
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

    card.componentSettings ??= modalServiceMock.componentSettings();
    card.iconData ??= modalServiceMock.iconSettings();

    expect(card.componentSettings).toEqual({
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
