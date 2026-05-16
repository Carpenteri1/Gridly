import { Injectable, inject } from '@angular/core';
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {CardModel} from "../../models/card.Model";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";
import { EditCardModel } from '../../models/editCard.Model';

@Injectable({
  providedIn: 'root'
})

export class CardEndpointService{
  private http = inject(HttpClient);


  delete(id: number) {
    return this.http.delete<CardModel>(UrlStringsUtil.CardUrlDelete+id).pipe(take(1));
  }

  get(): Observable<CardModel[]> {
    return this.http.get<CardModel[]>(UrlStringsUtil.CardUrlGet).pipe(take(1));
  }

  getById(id: number): Observable<CardModel> {
    return this.http.get<CardModel>(UrlStringsUtil.CardUrlGetById+id).pipe(take(1));
  }

  add(card: CardModel): Observable<CardModel> {
    return this.http.post<CardModel>(UrlStringsUtil.CardUrlSave, card).pipe(take(1));
  }

  edit(card: EditCardModel): Observable<CardModel> {
    return this.http.post<CardModel>(UrlStringsUtil.CardUrlEdit, card).pipe(take(1));
  }

  batchEdit(cards: CardModel[]): Observable<CardModel[]> {
    return this.http.post<CardModel[]>(UrlStringsUtil.CardsBatchUrlEdit, cards).pipe(take(1));
  }
}