import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { UrlStringsUtil } from '../../constants/url.strings.util';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ThirdPartyProvider } from '../../enums/third-party-provider.enum';

@Injectable({
  providedIn: 'root'
})
export class ProviderKeysEndpointService {
  private http = inject(HttpClient);

  getStatus(provider: ThirdPartyProvider): Observable<ProviderKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ProviderKeyStatusModel>(UrlStringsUtil.GetProviderKeyStatusUrl, { params }).pipe(take(1));
  }

  save(provider: ThirdPartyProvider, rawKey: string): Observable<void> {
    return this.http.post<void>(UrlStringsUtil.SaveProviderKeyUrl, { provider, rawKey }).pipe(take(1));
  }
}
