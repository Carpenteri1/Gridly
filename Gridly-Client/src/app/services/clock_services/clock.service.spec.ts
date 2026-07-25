import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ClockModel } from '../../models/clock.Model';
import { ClockEndpointService } from '../endpoint_services/clock.endpoint.service';
import { ClockService } from './clock.service';

describe('ClockService', () => {
  let service: ClockService;

  const cachedClock: ClockModel = { timeZone: 'Europe/Stockholm', utcOffsetSeconds: 7200, dstActive: true };
  const freshClock: ClockModel = { timeZone: 'Europe/Stockholm', utcOffsetSeconds: 3600, dstActive: false };

  const endpointMock = {
    get: jest.fn(),
    getTimeApiData: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();

    TestBed.configureTestingModule({
      providers: [
        ClockService,
        { provide: ClockEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(ClockService);
  });

  it('returns the cached clock data without calling the external-fetch endpoint', async () => {
    endpointMock.get.mockReturnValue(of(cachedClock));

    const result = await service.resolveClockData('Europe/Stockholm');

    expect(result).toEqual(cachedClock);
    expect(endpointMock.get).toHaveBeenCalledWith('Europe/Stockholm');
    expect(endpointMock.getTimeApiData).not.toHaveBeenCalled();
  });

  it('falls back to the external-fetch endpoint when nothing is cached', async () => {
    endpointMock.get.mockReturnValue(throwError(() => new Error('not found')));
    endpointMock.getTimeApiData.mockReturnValue(of(freshClock));

    const result = await service.resolveClockData('Europe/Stockholm');

    expect(result).toEqual(freshClock);
    expect(endpointMock.getTimeApiData).toHaveBeenCalledWith('Europe/Stockholm');
  });
});
