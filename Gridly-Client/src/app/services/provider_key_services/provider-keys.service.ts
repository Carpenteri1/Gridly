import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {BehaviorSubject, Observable, firstValueFrom} from 'rxjs';
import { ProviderKeysEndpointService } from '../endpoint_services/provider-keys.endpoint.service';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

@Injectable({ providedIn: 'root' })
export class ProviderKeysService {
  #api = inject(ProviderKeysEndpointService);

  private readonly statusSubject = new BehaviorSubject<ProviderKeyStatusModel | null>(null);
  private readonly status$: Observable<ProviderKeyStatusModel | null>;
  readonly currentStatus: Signal<ProviderKeyStatusModel | null | undefined>;

  private readonly promptSubject = new BehaviorSubject<boolean>(false);
  readonly shouldPromptForKey$: Observable<boolean>;
  readonly shouldPromptForKey: Signal<boolean | undefined>;

  constructor() {
    this.status$ = this.statusSubject.asObservable();
    this.currentStatus = toSignal(this.status$);
    this.shouldPromptForKey$ = this.promptSubject.asObservable();
    this.shouldPromptForKey = toSignal(this.shouldPromptForKey$);
  }

  private getStatus$ = (provider: ThirdPartyProvider) => this.#api.getStatus(provider);
  getStatus = (provider: ThirdPartyProvider) => firstValueFrom(this.getStatus$(provider));

  save(rawKey: string, provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): Observable<void> {
    return this.#api.save(provider, rawKey);
  }

  onKeySaved(provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): void {
    this.promptSubject.next(false);
    this.getStatus(provider);
  }

  promptForInvalidKey(): void {
    this.promptSubject.next(true);
  }
}
