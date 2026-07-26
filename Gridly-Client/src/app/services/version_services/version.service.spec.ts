import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { VersionEndpointService } from '../endpoint_services/version.endpoint.service';
import { VersionService } from './version.service';

describe('VersionService', () => {
  const version = { tag_name: 'v1.2.3', newRelease: false };
  const endpointMock = {
    get: jest.fn(() => of(version)),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    TestBed.configureTestingModule({
      providers: [
        VersionService,
        { provide: VersionEndpointService, useValue: endpointMock },
      ],
    });
  });

  it('loads the current version through the endpoint service', () => {
    const service = TestBed.inject(VersionService);

    expect(endpointMock.get).toHaveBeenCalledTimes(1);
    expect(service.currentVersion()).toEqual(version);
  });

  it('resolves to undefined when the endpoint call fails', () => {
    TestBed.overrideProvider(VersionEndpointService, {
      useValue: { get: jest.fn(() => throwError(() => new Error('network error'))) },
    });

    const service = TestBed.inject(VersionService);

    expect(service.currentVersion()).toBeUndefined();
  });
});
