import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ModalService } from '../../../Services/modal.service';
import { CardModel } from '../../../Models/Card.Model';
import { IconModel } from '../../../Models/Icon.Model';
import { AddWidgetModalComponent } from './add-widget-modal.component';
import { WidgetType } from '../../../Types/widget.type.enum';

describe('AddWidgetModalComponent', () => {
  let fixture: ComponentFixture<AddWidgetModalComponent>;
  let component: AddWidgetModalComponent;

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
      imports: [AddWidgetModalComponent],
      providers: [{ provide: ModalService, useValue: modalServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(AddWidgetModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('emits a new widget payload for both supported widget types', () => {
    const emitSpy = jest.spyOn(component.newWidget, 'emit');

    component.onSelect(WidgetType.Empty);
    component.onSelect(WidgetType.Custom);

    expect(emitSpy).toHaveBeenCalledTimes(2);
    expect(emitSpy.mock.calls[0][0]).toBeDefined();
    expect(emitSpy.mock.calls[1][0]).toBeDefined();
  });

  it('emits a fully initialized widget model', () => {
    const emitSpy = jest.spyOn(component.newWidget, 'emit');

    component.onSelect(WidgetType.Empty);

    const widget = emitSpy.mock.calls[0]?.[0] as CardModel | undefined;

    expect(widget).toBeDefined();

    if (!widget) {
      throw new Error('Expected widget payload to be emitted.');
    }

    widget.componentSettings ??= modalServiceMock.componentSettings();
    widget.iconData ??= modalServiceMock.iconSettings();

    expect(widget.componentSettings).toEqual({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    });
    expect(widget.iconData).toEqual({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'add_box',
    });
  });
});
