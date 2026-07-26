import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ProviderKeysService } from './provider-keys.service';
import { ProviderKeysEndpointService } from '../endpoint_services/provider-keys.endpoint.service';
import { ProviderKeyStatus } from '../../enums/provider-key-status.enum';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

describe('ProviderKeysService', () => {
  const endpointMock = {
    getStatus: jest.fn(),
    save: jest.fn(),
  };

  const createService = () => {
    TestBed.configureTestingModule({
      providers: [
        ProviderKeysService,
        { provide: ProviderKeysEndpointService, useValue: endpointMock },
      ],
    });

    return TestBed.inject(ProviderKeysService);
  };

  beforeEach(() => {
    jest.clearAllMocks();
    TestBed.resetTestingModule();
  });

  it('fetches the status on creation so a reload reflects the stored key', () => {
    endpointMock.getStatus.mockReturnValue(of({ exists: true, status: ProviderKeyStatus.Valid }));

    const service = createService();

    expect(endpointMock.getStatus).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing);
    expect(service.currentStatus()).toEqual({ exists: true, status: ProviderKeyStatus.Valid });
  });

  it('leaves the status null when the request fails', () => {
    endpointMock.getStatus.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 500 })),
    );

    const service = createService();

    expect(service.currentStatus()).toBeNull();
  });

  it('refreshes the status after a key is saved', () => {
    endpointMock.getStatus.mockReturnValue(of({ exists: false, status: ProviderKeyStatus.Unknown }));
    const service = createService();
    expect(endpointMock.getStatus).toHaveBeenCalledTimes(1);

    endpointMock.getStatus.mockReturnValue(of({ exists: true, status: ProviderKeyStatus.Valid }));
    service.onKeySaved(ThirdPartyProvider.VisualCrossing);

    expect(endpointMock.getStatus).toHaveBeenCalledTimes(2);
    expect(service.currentStatus()).toEqual({ exists: true, status: ProviderKeyStatus.Valid });
  });
});
