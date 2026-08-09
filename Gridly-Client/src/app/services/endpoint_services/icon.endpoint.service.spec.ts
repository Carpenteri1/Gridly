import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { IconModel } from '../../models/icon.Model';
import { SearchIconsResultDto } from '../../dtos/searchIconsResultDto';
import { IconEndpointService } from './icon.endpoint.service';

describe('IconEndpointService', () => {
  let service: IconEndpointService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(IconEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the icon endpoint and returns the response', () => {
    const icon: IconModel = { type: 'svg', name: 'dashboard', base64Data: 'abc', materialIcon: 'dashboard' };
    let result: IconModel | undefined;

    service.get().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.icon.get);
    expect(req.request.method).toBe('GET');
    req.flush(icon);

    expect(result).toEqual(icon);
  });

  it('sends a GET request to the icon search endpoint with the search input appended to the URL', () => {
    const dto: SearchIconsResultDto = { icons: ['box', 'grid'] };
    let result: SearchIconsResultDto | undefined;

    service.search('box').subscribe((res) => (result = res));

    const req = httpMock.expectOne(`${urlConstants.icon.search}box`);
    expect(req.request.method).toBe('GET');
    req.flush(dto);

    expect(result).toEqual(dto);
  });
});
