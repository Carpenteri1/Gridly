import {Injectable} from '@angular/core';
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {ComponentModel} from "../../Models/Component.Model";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})

export class ComponentEndpointService{

  constructor(private http: HttpClient) {}

  delete(id: number): Observable<ComponentModel> {
    return this.http.delete<ComponentModel>(UrlStringsUtil.ComponentUrlDelete+id).pipe(take(1));
  }

  get(): Observable<ComponentModel[]> {
    return this.http.get<ComponentModel[]>(UrlStringsUtil.ComponentUrlGet).pipe(take(1));
  }

  getById(id: number): Observable<ComponentModel> {
    return this.http.get<ComponentModel>(UrlStringsUtil.ComponentUrlGetById+id).pipe(take(1));
  }

  add(newComponent: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlSave, newComponent).pipe(take(1));
  }

  edit(componentModel: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlEdit, componentModel).pipe(take(1));
  }

  batchEdit(editedComponent: ComponentModel[]): Observable<ComponentModel[]> {
    return this.http.post<ComponentModel[]>(UrlStringsUtil.ComponentsBatchUrlEdit, editedComponent).pipe(take(1));
  }
}
