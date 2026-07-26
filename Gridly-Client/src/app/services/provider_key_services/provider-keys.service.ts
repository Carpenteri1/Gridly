import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject, Observable, catchError, of, take, tap } from 'rxjs';
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
    this.refreshStatus();
  }

  refreshStatus(provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): void {
    this.#api.getStatus(provider).pipe(
      take(1),
      catchError(() => of(null)),
      tap((status) => this.statusSubject.next(status)),
    ).subscribe();
  }

  save(rawKey: string, provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): Observable<void> {
    return this.#api.save(provider, rawKey);
  }

  onKeySaved(provider: ThirdPartyProvider = ThirdPartyProvider.VisualCrossing): void {
    this.promptSubject.next(false);
    this.refreshStatus(provider);
  }

  promptForInvalidKey(): void {
    this.promptSubject.next(true);
  }
}
