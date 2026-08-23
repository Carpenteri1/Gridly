import { TestBed } from '@angular/core/testing';
import { EMPTY, of } from 'rxjs';
import { ProviderKeysService } from './provider-keys.service';
import { ProviderKeysEndpointService } from '../endpoint_services/provider-keys.endpoint.service';
import { ProviderKeyStatus } from '../../enums/provider-key-status.enum';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

describe('ProviderKeysService', () => {
  const endpointMock = {
    getLocalProviderStatus: jest.fn(),
    getRemoteProviderStatus: jest.fn(),
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
    endpointMock.getLocalProviderStatus.mockReturnValue(of({ exists: true, status: ProviderKeyStatus.Valid }));

    const service = createService();

    expect(endpointMock.getLocalProviderStatus).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing);
    expect(service.currentStatus()).toEqual({ exists: true, status: ProviderKeyStatus.Valid });
  });

  it('leaves the status undefined when the local status request emits no value', () => {
    endpointMock.getLocalProviderStatus.mockReturnValue(EMPTY);

    const service = createService();

    expect(service.currentStatus()).toBeUndefined();
  });

  it('refreshes the status after a key is saved', async () => {
    endpointMock.getLocalProviderStatus.mockReturnValue(of({ exists: false, status: ProviderKeyStatus.Unknown }));
    const service = createService();
    expect(endpointMock.getLocalProviderStatus).toHaveBeenCalledTimes(1);

    endpointMock.getRemoteProviderStatus.mockReturnValue(of({ exists: true, status: ProviderKeyStatus.Valid }));
    await service.onKeySaved(ThirdPartyProvider.VisualCrossing);

    expect(endpointMock.getRemoteProviderStatus).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing);
    expect(service.currentStatus()).toEqual({ exists: true, status: ProviderKeyStatus.Valid });
  });

  it('saves a key for the given provider', () => {
    endpointMock.getLocalProviderStatus.mockReturnValue(EMPTY);
    endpointMock.save.mockReturnValue(of(undefined));
    const service = createService();

    service.save('raw-key', ThirdPartyProvider.VisualCrossing);

    expect(endpointMock.save).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing, 'raw-key');
  });

  it('saves a key for the default provider when none is given', () => {
    endpointMock.getLocalProviderStatus.mockReturnValue(EMPTY);
    endpointMock.save.mockReturnValue(of(undefined));
    const service = createService();

    service.save('raw-key');

    expect(endpointMock.save).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing, 'raw-key');
  });

  it('flags that the user should be prompted for a key', () => {
    endpointMock.getLocalProviderStatus.mockReturnValue(EMPTY);
    const service = createService();

    service.promptForInvalidKey();

    expect(service.shouldPromptForKey()).toBe(true);
  });
});
