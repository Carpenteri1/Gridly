import { Injectable, inject } from '@angular/core';
import {urlConstants} from "../../constants/url.constants";
import {CardModel} from "../../models/card.Model";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})

export class CardEndpointService{
  private http = inject(HttpClient);

  get(): Observable<CardModel[]> {
    return this.http.get<CardModel[]>(urlConstants.card.get).pipe(take(1));
  }
}
