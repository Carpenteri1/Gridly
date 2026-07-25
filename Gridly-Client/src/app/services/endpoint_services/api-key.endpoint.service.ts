import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { UrlStringsUtil } from '../../constants/url.strings.util';
import { ApiKeyStatusModel } from '../../models/apiKeyStatus.Model';
import { ThirdPartyProvider } from '../../types/third-party-provider.enum';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyEndpointService {
  private http = inject(HttpClient);

  getStatus(provider: ThirdPartyProvider): Observable<ApiKeyStatusModel> {
    const params = new HttpParams().set('Provider', provider);
    return this.http.get<ApiKeyStatusModel>(UrlStringsUtil.GetApiKeyStatusUrl, { params }).pipe(take(1));
  }

  save(provider: ThirdPartyProvider, rawKey: string): Observable<void> {
    return this.http.post<void>(UrlStringsUtil.SaveApiKeyUrl, { provider, rawKey }).pipe(take(1));
  }
}
