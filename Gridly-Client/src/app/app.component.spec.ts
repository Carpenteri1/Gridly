import { TestBed } from '@angular/core/testing';
import { TranslateService } from '@ngx-translate/core';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  const translateServiceMock = {
    instant: (key: string) => (key === 'app.text.title' ? 'Gridly' : key),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: TranslateService, useValue: translateServiceMock }],
    });
  });

  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('uses the shared client title', () => {
    const app = TestBed.runInInjectionContext(() => new AppComponent());

    expect(app.title).toBe('Gridly');
  });

  it('starts with edit mode disabled', () => {
    const app = TestBed.runInInjectionContext(() => new AppComponent());

    expect(app.isEditMode).toBe(false);
  });
});
