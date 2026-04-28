import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ComponentModel } from '../../Models/Component.Model';
import { ComponentService } from '../../Services/component.service';
import { GridService } from '../../Services/grid.service';
import { VersionService } from '../../Services/version.service';
import { HeaderComponent } from './header.component';

type HeaderComponentTestHarness = HeaderComponent & {
  add(component: ComponentModel): void;
  setEditMode(): void;
};

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;
  let component: HeaderComponent;

  const componentServiceMock = {
    add: jest.fn(),
    component$: of(undefined),
  };

  const versionServiceMock = {
    version$: of({ id: 1, name: 'v1.0.0' }),
  };

  const gridServiceMock = {
    toggle: jest.fn(),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        { provide: ComponentService, useValue: componentServiceMock },
        { provide: VersionService, useValue: versionServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('toggles the menu signal through the public method', () => {
    expect(component.showMenu()).toBe(false);

    component.toggleMenu();
    expect(component.showMenu()).toBe(true);

    component.toggleMenu();
    expect(component.showMenu()).toBe(false);
  });

  it('delegates add and edit mode actions to the injected services', () => {
    const newComponent = new ComponentModel();

    (component as HeaderComponentTestHarness).add(newComponent);
    (component as HeaderComponentTestHarness).setEditMode();

    expect(componentServiceMock.add).toHaveBeenCalledWith(newComponent);
    expect(gridServiceMock.toggle).toHaveBeenCalled();
  });

  it('renders the client title', () => {
    expect((fixture.nativeElement as HTMLElement).querySelector('h1')?.textContent).toContain('Gridly');
  });
});
