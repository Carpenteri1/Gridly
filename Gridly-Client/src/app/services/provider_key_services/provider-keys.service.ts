import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {BehaviorSubject, Observable, firstValueFrom, take} from 'rxjs';
import { ProviderKeysEndpointService } from '../endpoint_services/provider-keys.endpoint.service';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

@Injectable({ providedIn: 'root' })
export class ProviderKeysService {
  #api = inject(ProviderKeysEndpointService);

  private readonly statusSubject = new BehaviorSubject<ProviderKeyStatusModel | undefined>(undefined);
  private readonly status$: Observable<ProviderKeyStatusModel | undefined>;
  readonly currentStatus: Signal<ProviderKeyStatusModel | null | undefined>;

  private readonly promptSubject = new BehaviorSubject<boolean>(false);
  readonly shouldPromptForKey$: Observable<boolean>;
  readonly shouldPromptForKey: Signal<boolean | undefined>;

  constructor() {
    this.status$ = this.statusSubject.asObservable();
    this.currentStatus = toSignal(this.status$);
    this.shouldPromptForKey$ = this.promptSubject.asObservable();
    this.shouldPromptForKey = toSignal(this.shouldPromptForKey$);
    this.refresh();
  }

  refresh(): void {
    this.#api.getLocalProviderStatus(ThirdPartyProvider.VisualCrossing).pipe(take(1))
      .subscribe((status) =>
        this.statusSubject.next(status));
  }

  refreshStatus$ = (provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing) => this.#api.getRemoteProviderStatus(provider);
  refreshStatus = async (provider: ThirdPartyProvider) => {
    const providerStatus = await firstValueFrom(this.refreshStatus$(provider));
    this.statusSubject.next(providerStatus);
    return providerStatus;
  };
  save(rawKey: string, provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): Observable<void> {
    return this.#api.save(provider, rawKey);
  }

  async onKeySaved(provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): Promise<void> {
    this.promptSubject.next(false);
    await this.refreshStatus(provider);
  }

  promptForInvalidKey(): void {
    this.promptSubject.next(true);
  }
}
