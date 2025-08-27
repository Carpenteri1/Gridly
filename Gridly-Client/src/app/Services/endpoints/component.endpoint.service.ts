import {Injectable} from '@angular/core';
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {ComponentModel} from "../../Models/Component.Model";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {MapEditComponentData} from "../../Utils/editComponentModal.factory";

@Injectable({
  providedIn: 'root'
})

export class ComponentEndpointService{
  private flexItems: ComponentModel[] = [];

  constructor(private http: HttpClient) {}

  Delete(id: number): Observable<ComponentModel> {
    return this.http.delete<ComponentModel>(UrlStringsUtil.ComponentUrlDelete+id);
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

  EditComponent(componentModel: ComponentModel, selected: number): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(UrlStringsUtil.ComponentUrlEdit, MapEditComponentData(componentModel, selected));
  }

  EditComponents(editedComponent: ComponentModel[]): Observable<ComponentModel[]> {
    return this.http.post<ComponentModel[]>(UrlStringsUtil.ComponentsBatchUrlEdit, editedComponent);
  }

  GetIndex(id: number): number{
    return this.flexItems === null ? -1 : this.flexItems.findIndex(item => item.id == id);
  }
}
