import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { IconEndpointService } from '../endpoint_services/icon.endpoint.service';
import { IconService } from './Icon.service';

describe('IconService', () => {
  let service: IconService;

  const endpointMock = {
    search: jest.fn(),
  };

  beforeEach(() => {
    jest.useFakeTimers();
    jest.clearAllMocks();
    endpointMock.search.mockReturnValue(of({ icons: ['home', 'dashboard'] }));

    TestBed.configureTestingModule({
      providers: [
        IconService,
        { provide: IconEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(IconService);
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it('debounces and trims icon searches before calling the endpoint', () => {
    const results: unknown[] = [];
    const subscription = service.icons$.subscribe((value) => results.push(value));

    service.search('  home  ');
    jest.advanceTimersByTime(299);
    expect(endpointMock.search).not.toHaveBeenCalled();

    jest.advanceTimersByTime(1);
    expect(endpointMock.search).toHaveBeenCalledWith('home');
    expect(results.at(-1)).toEqual({ icons: ['home', 'dashboard'] });

    subscription.unsubscribe();
  });

  it('does not call the endpoint for empty searches', () => {
    const results: unknown[] = [];
    const subscription = service.icons$.subscribe((value) => results.push(value));

    service.search('   ');
    jest.advanceTimersByTime(300);

    expect(endpointMock.search).not.toHaveBeenCalled();
    expect(results.at(-1)).toBeNull();

    subscription.unsubscribe();
  });

  it('converts endpoint errors to null results', () => {
    endpointMock.search.mockReturnValueOnce(throwError(() => new Error('search failed')));
    const results: unknown[] = [];
    const subscription = service.icons$.subscribe((value) => results.push(value));

    service.search('broken');
    jest.advanceTimersByTime(300);

    expect(results.at(-1)).toBeNull();
    subscription.unsubscribe();
  });
});
