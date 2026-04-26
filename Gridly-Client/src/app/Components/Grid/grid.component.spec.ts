import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ComponentModel } from '../../Models/Component.Model';
import { ComponentService } from '../../Services/component.service';
import { GridService } from '../../Services/grid.service';
import { GridComponent } from './grid.component';

type GridComponentTestHarness = GridComponent & {
  Drop(event: unknown): void;
};

describe('GridComponent', () => {
  let fixture: ComponentFixture<GridComponent>;
  let component: GridComponent;
  let components: ComponentModel[];

  const componentServiceMock = {
    components$: of([] as ComponentModel[]),
  };

  const gridServiceMock = {
    getEditMode: jest.fn(() => true),
  };

  beforeEach(async () => {
    components = [
      { id: 1, indexPosition: 1, name: 'One', url: 'https://one.example' },
      { id: 2, indexPosition: 2, name: 'Two', url: 'https://two.example' },
    ];
    componentServiceMock.components$ = of(components);

    await TestBed.configureTestingModule({
      imports: [GridComponent],
      providers: [
        { provide: ComponentService, useValue: componentServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(GridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders one item component per returned component', () => {
    const items = fixture.nativeElement.querySelectorAll('app-item');

    expect(items).toHaveLength(2);
  });

  it('reorders items when drag-drop happens in edit mode', () => {
    const event = {
      item: { data: components },
      previousIndex: 0,
      currentIndex: 1,
    } as never;

    (component as GridComponentTestHarness).Drop(event);

    expect(components.map((item) => item.id)).toEqual([2, 1]);
  });

  it('does not reorder items when edit mode is disabled', () => {
    component.inEditMode = false;
    const event = {
      item: { data: components },
      previousIndex: 0,
      currentIndex: 1,
    } as never;

    (component as GridComponentTestHarness).Drop(event);

    expect(components.map((item) => item.id)).toEqual([1, 2]);
  });
});
