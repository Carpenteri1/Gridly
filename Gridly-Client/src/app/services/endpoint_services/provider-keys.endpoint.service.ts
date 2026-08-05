import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { urlConstants } from '../../constants/url.constants';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

@Injectable({
  providedIn: 'root'
})
export class ProviderKeysEndpointService {
  private http = inject(HttpClient);

  getLocalProviderStatus(provider: ThirdPartyProvider): Observable<ProviderKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ProviderKeyStatusModel>(urlConstants.providerKey.getLocalStatus, { params }).pipe(take(1));
  }

  getRemoteProviderStatus(provider: ThirdPartyProvider): Observable<ProviderKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ProviderKeyStatusModel>(urlConstants.providerKey.getRemoteStatus, { params }).pipe(take(1));
  }

  save(provider: ThirdPartyProvider, rawKey: string): Observable<void> {
    return this.http.post<void>(urlConstants.providerKey.save, { provider, rawKey }).pipe(take(1));
  }
}
