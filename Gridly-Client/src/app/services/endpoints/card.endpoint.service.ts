import { Injectable, inject } from '@angular/core';
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {CardModel} from "../../models/card.Model";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";
import { EditCardModel } from '../../models/editCard.Model';

@Injectable({
  providedIn: 'root'
})

export class CardEnpointService{
  private http = inject(HttpClient);


  delete(id: number) {
    return this.http.delete<CardModel>(UrlStringsUtil.ComponentUrlDelete+id).pipe(take(1));
  }

  get(): Observable<CardModel[]> {
    return this.http.get<CardModel[]>(UrlStringsUtil.ComponentUrlGet).pipe(take(1));
  }

  getById(id: number): Observable<CardModel> {
    return this.http.get<CardModel>(UrlStringsUtil.ComponentUrlGetById+id).pipe(take(1));
  }

  add(newComponent: CardModel): Observable<CardModel> {
    return this.http.post<CardModel>(UrlStringsUtil.ComponentUrlSave, newComponent).pipe(take(1));
  }

  edit(componentModel: EditCardModel): Observable<CardModel> {
    return this.http.post<CardModel>(UrlStringsUtil.ComponentUrlEdit, componentModel).pipe(take(1));
  }

  batchEdit(editedComponent: EditCardModel[]): Observable<CardModel[]> {
    return this.http.post<CardModel[]>(UrlStringsUtil.ComponentsBatchUrlEdit, editedComponent).pipe(take(1));
  }
}

export { CardEnpointService as ComponentEndpointService };
