import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ProviderKeyStatus } from '../../enums/provider-key-status.enum';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';
import { ProviderKeysEndpointService } from './provider-keys.endpoint.service';

describe('ProviderKeysEndpointService', () => {
  let service: ProviderKeysEndpointService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ProviderKeysEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request for the local provider key status with the provider as a param', () => {
    const status: ProviderKeyStatusModel = { exists: true, keyStatus: ProviderKeyStatus.Valid };
    let result: ProviderKeyStatusModel | undefined;

    service.getLocalProviderStatus(ThirdPartyProvider.VisualCrossing).subscribe((res) => (result = res));

    const req = httpMock.expectOne(
      (r) => r.url === urlConstants.providerKey.getLocalStatus && r.params.get('Provider') === ThirdPartyProvider.VisualCrossing
    );
    expect(req.request.method).toBe('GET');
    req.flush(status);

    expect(result).toEqual(status);
  });

  it('sends a GET request for the remote provider key status with the provider as a param', () => {
    const status: ProviderKeyStatusModel = { exists: false, keyStatus: ProviderKeyStatus.Unknown };
    let result: ProviderKeyStatusModel | undefined;

    service.getRemoteProviderStatus(ThirdPartyProvider.VisualCrossing).subscribe((res) => (result = res));

    const req = httpMock.expectOne(
      (r) => r.url === urlConstants.providerKey.getRemoteStatus && r.params.get('Provider') === ThirdPartyProvider.VisualCrossing
    );
    expect(req.request.method).toBe('GET');
    req.flush(status);

    expect(result).toEqual(status);
  });

  it('sends a POST request to save the provider key with the provider and raw key as the body', () => {
    let completed = false;

    service.save(ThirdPartyProvider.VisualCrossing, 'raw-key').subscribe(() => (completed = true));

    const req = httpMock.expectOne(urlConstants.providerKey.save);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ provider: ThirdPartyProvider.VisualCrossing, rawKey: 'raw-key' });
    req.flush(null);

    expect(completed).toBe(true);
  });
});
