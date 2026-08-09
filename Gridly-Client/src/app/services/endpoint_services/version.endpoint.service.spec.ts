import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { VersionModel } from '../../models/version.Model';
import { VersionEndpointService } from './version.endpoint.service';

describe('VersionEndpointService', () => {
  let service: VersionEndpointService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(VersionEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the version endpoint and returns the response', () => {
    const version: VersionModel = { name: 'v1.2.3', newRelease: false };
    let result: VersionModel | undefined;

    service.get().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.version.get);
    expect(req.request.method).toBe('GET');
    req.flush(version);

    expect(result).toEqual(version);
  });
});
