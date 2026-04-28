import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddWidgetModalComponent } from './add-widget-modal.component';
import { WidgetType } from '../../../Types/widget.type.enum';

describe('AddWidgetModalComponent', () => {
  let fixture: ComponentFixture<AddWidgetModalComponent>;
  let component: AddWidgetModalComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddWidgetModalComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AddWidgetModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('emits false when the open state is cleared from the modal', () => {
    const emitSpy = jest.spyOn(component.openChange, 'emit');

    component.handleOpenChange();

    expect(emitSpy).toHaveBeenCalledWith(false);
  });

  it('emits a new widget payload for both supported widget types', () => {
    const openChangeSpy = jest.spyOn(component.openChange, 'emit');
    const emitSpy = jest.spyOn(component.newWidget, 'emit');

    component.onSelect(WidgetType.Empty);
    component.onSelect(WidgetType.Custom);

    expect(openChangeSpy).toHaveBeenCalledTimes(2);
    expect(openChangeSpy).toHaveBeenNthCalledWith(1, false);
    expect(openChangeSpy).toHaveBeenNthCalledWith(2, false);
    expect(emitSpy).toHaveBeenCalledTimes(2);
    expect(emitSpy.mock.calls[0][0]).toBeDefined();
    expect(emitSpy.mock.calls[1][0]).toBeDefined();
  });
});
