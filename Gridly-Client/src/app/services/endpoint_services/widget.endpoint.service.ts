import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {urlConstants} from "../../constants/url.constants";
import {Observable, take} from "rxjs";
import {Widget} from "../../interfaces/widget.Interface";

@Injectable({
  providedIn: 'root'
})

export class WidgetEndpointService{
  private http = inject(HttpClient);
  get(): Observable<Widget[]> {
    return this.http.get<Widget[]>(urlConstants.widget.get).pipe(take(1));
  }
}
