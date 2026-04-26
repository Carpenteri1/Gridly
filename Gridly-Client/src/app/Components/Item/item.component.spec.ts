import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ComponentModel } from '../../Models/Component.Model';
import { ComponentService } from '../../Services/component.service';
import { GridService } from '../../Services/grid.service';
import { ItemComponent } from './item.component';

type ItemComponentTestHarness = ItemComponent & {
  edit(component: ComponentModel): void;
  remove(id: number): void;
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
    component$: of(currentComponent),
    currentComponent: jest.fn(() => currentComponent),
    currentComponents: jest.fn(() => [currentComponent]),
    delete: jest.fn(),
    edit: jest.fn(),
  };

  const gridServiceMock = {
    getEditMode: jest.fn(() => true),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ItemComponent],
      providers: [
        { provide: ComponentService, useValue: componentServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ItemComponent);
    component = fixture.componentInstance;
    component.id = 7;
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
});
