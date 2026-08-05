import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { LocaleStringsService } from '../locale_services/locale-strings.service';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

@Injectable({
  providedIn: 'root'
})
export class ProviderKeysEndpointService {
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);

  getLocalProviderStatus(provider: ThirdPartyProvider): Observable<ProviderKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ProviderKeyStatusModel>(this.localeStrings.locale.providerKey.url.getLocalStatus, { params }).pipe(take(1));
  }

  getRemoteProviderStatus(provider: ThirdPartyProvider): Observable<ProviderKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ProviderKeyStatusModel>(this.localeStrings.locale.providerKey.url.getRemoteStatus, { params }).pipe(take(1));
  }

  save(provider: ThirdPartyProvider, rawKey: string): Observable<void> {
    return this.http.post<void>(this.localeStrings.locale.providerKey.url.save, { provider, rawKey }).pipe(take(1));
  }
}
