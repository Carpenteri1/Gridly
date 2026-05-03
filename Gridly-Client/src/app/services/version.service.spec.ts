import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { VersionEndpointService } from './endpoints/version.endpoint.service';
import { VersionService } from './version.service';

describe('VersionService', () => {
  const version = { id: 1, name: 'v1.2.3' };
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
});
