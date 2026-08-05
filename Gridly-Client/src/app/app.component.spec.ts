import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { LocaleStringsService } from './services/locale_services/locale-strings.service';

describe('AppComponent', () => {
  const localeStringsServiceMock = {
    locale: {
      app: { text: { title: 'Gridly' } },
    },
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: LocaleStringsService, useValue: localeStringsServiceMock }],
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
