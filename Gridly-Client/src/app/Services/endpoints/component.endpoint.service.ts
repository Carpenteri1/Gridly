import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from "rxjs";
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {ComponentModel} from "../../Models/Component.Model";
import {IconModel} from "../../Models/Icon.Model";

@Injectable({
  providedIn: 'root'
})

export class ComponentEndpointService{
  private flexItems: ComponentModel[] = [];
  isLoading = false;

  constructor(private http: HttpClient) {}

  RemoveComponent(id: number) {
    this.http.delete<ComponentModel>(UrlStringsUtil.ComponentUrlDelete+id);
  }

  GetComponents(): Observable<ComponentModel[]> {
    return this.http.get<ComponentModel[]>(UrlStringsUtil.ComponentUrlGet);
  }

  GetComponentById(id: number): Observable<ComponentModel> {
    return this.http.get<ComponentModel>(UrlStringsUtil.ComponentUrlGetById+id);
  }

  AddComponent(newComponent: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlSave, newComponent);
  }

  EditComponent(editedComponent: ComponentModel, editIconData?: IconModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlEdit, {editedComponent, editIconData});
  }

  GetIndex(id: number): number{
    return this.flexItems === null ? -1 : this.flexItems.findIndex(item => item.id == id);
  }
}
