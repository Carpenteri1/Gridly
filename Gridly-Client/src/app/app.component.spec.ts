import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AppComponent } from './app.component';
import { TextStringsUtil } from './Constants/text.strings.util';
import { ComponentEndpointService } from './Services/endpoints/component.endpoint.service';

describe('AppComponent', () => {
  const componentEndpointServiceMock = {
    add: jasmine.createSpy('add').and.returnValue(of({})),
    batchEdit: jasmine.createSpy('batchEdit').and.returnValue(of([])),
    delete: jasmine.createSpy('delete').and.returnValue(of({})),
    edit: jasmine.createSpy('edit').and.returnValue(of({})),
    get: jasmine.createSpy('get').and.returnValue(of([])),
    getById: jasmine.createSpy('getById').and.returnValue(of({})),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        { provide: ComponentEndpointService, useValue: componentEndpointServiceMock },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have the 'Gridly' title`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual(TextStringsUtil.ClientTitle);
  });

  it('should render the header title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain(TextStringsUtil.ClientTitle);
  });
});
