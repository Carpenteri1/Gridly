import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {Observable, take} from "rxjs";
import {Widget} from "../../interfaces/widget.Interface";

@Injectable({
  providedIn: 'root'
})

export class WidgetEndpointService{
  private http = inject(HttpClient);
  get(): Observable<Widget[]> {
    return this.http.get<Widget[]>(UrlStringsUtil.GetWidgetUrl).pipe(take(1));
  }
}
