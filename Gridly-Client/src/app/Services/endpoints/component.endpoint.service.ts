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

  Delete(id: number): Observable<ComponentModel> {
    return this.http.delete<ComponentModel>(UrlStringsUtil.ComponentUrlDelete+id).pipe(take(1));
  }

  GetComponents(): Observable<ComponentModel[]> {
    return this.http.get<ComponentModel[]>(UrlStringsUtil.ComponentUrlGet).pipe(take(1));
  }

  GetComponentById(id: number): Observable<ComponentModel> {
    return this.http.get<ComponentModel>(UrlStringsUtil.ComponentUrlGetById+id).pipe(take(1));
  }

  AddComponent(newComponent: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlSave, newComponent).pipe(take(1));
  }

  EditComponent(componentModel: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlEdit, componentModel).pipe(take(1));
  }

  EditComponents(editedComponent: ComponentModel[]): Observable<ComponentModel[]> {
    return this.http.post<ComponentModel[]>(UrlStringsUtil.ComponentsBatchUrlEdit, editedComponent).pipe(take(1));
  }
}
