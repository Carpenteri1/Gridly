import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ComponentModel } from '../../Models/Component.Model';
import { ComponentService } from '../../Services/component.service';
import { ComponentRulesService } from '../../Services/component-rules.service';
import { GridService } from '../../Services/grid.service';
import { ItemComponent } from './item.component';

type ItemComponentTestHarness = ItemComponent & {
  edit(component: ComponentModel): void;
  remove(id: number): void;
  hasMaterialIcon(item: ComponentModel): boolean;
};

describe('ItemComponent', () => {
  let fixture: ComponentFixture<ItemComponent>;
  let component: ItemComponent;

  const currentComponent: ComponentModel = {
    id: 7,
    indexPosition: 1,
    name: 'Weather',
    url: 'https://weather.example',
    iconData: {
      name: '',
      type: '',
      base64Data: '',
      materialIcon: 'cloud',
    },
    componentSettings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  const componentServiceMock = {
    currentComponents: jest.fn(() => [currentComponent]),
    delete: jest.fn(),
    edit: jest.fn(),
  };

  const componentRulesServiceMock = {
    hasMaterialIcon: jest.fn(() => true),
  };

  const editMode = signal(true);
  const gridServiceMock = { editMode: editMode.asReadonly() };

  beforeEach(async () => {
    jest.clearAllMocks();

    await TestBed.configureTestingModule({
      imports: [ItemComponent],
      providers: [
        { provide: ComponentService, useValue: componentServiceMock },
        { provide: ComponentRulesService, useValue: componentRulesServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ItemComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('component', currentComponent);
    fixture.detectChanges();
  });

  it('renders the current component name and material icon', () => {
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('mat-icon')?.textContent).toContain('cloud');
    expect(element.textContent).toContain('Weather');
  });

  it('opens the edit and delete dialogs from the component methods', () => {
    component.openEditDialog();
    component.openDeleteDialog();

    expect(component.isEditModalOpen).toBe(true);
    expect(component.isDeleteModalOpen).toBe(true);
  });

  it('closes both dialogs when the matching modal id is emitted', () => {
    component.isEditModalOpen = true;
    component.isDeleteModalOpen = true;

    component.handleModalChange(7);

    expect(component.isEditModalOpen).toBe(false);
    expect(component.isDeleteModalOpen).toBe(false);
  });

  it('delegates edit and remove actions to the component service', () => {
    (component as ItemComponentTestHarness).edit(currentComponent);
    (component as ItemComponentTestHarness).remove(7);

    expect(componentServiceMock.edit).toHaveBeenCalledWith(currentComponent);
    expect(componentServiceMock.delete).toHaveBeenCalledWith(7);
  });
  
  it('hasMaterialIcon returns the value from the component rules service', () => {
    const result = (component as ItemComponentTestHarness).hasMaterialIcon(currentComponent);
    expect(componentRulesServiceMock.hasMaterialIcon).toHaveBeenCalledWith(currentComponent);
    expect(result).toBe(true);
  });

});
