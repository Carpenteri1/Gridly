import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from "rxjs";
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {IComponentModel} from "../../Models/IComponent.Model";
import {IIconModel} from "../../Models/IIcon.Model";

@Injectable({
  providedIn: 'root'
})

export class ComponentEndpointService{
  private flexItems: IComponentModel[] = [];
  isLoading = false;

  constructor(private http: HttpClient) {}

  RemoveComponent(id: number) {
    this.http.delete<IComponentModel>(UrlStringsUtil.ComponentUrlDelete+id);
  }

  GetComponents(): Observable<IComponentModel[]> {
    return this.http.get<IComponentModel[]>(UrlStringsUtil.ComponentUrlGet);
  }

  GetComponentById(id: number): Observable<IComponentModel> {
    return this.http.get<IComponentModel>(UrlStringsUtil.ComponentUrlGetById+id);
  }

  AddComponent(newComponent: IComponentModel): Observable<IComponentModel> {
    return this.http.post<IComponentModel>(UrlStringsUtil.ComponentUrlSave, newComponent);
  }

  EditComponent(editedComponent: IComponentModel, editIconData?: IIconModel): Observable<IComponentModel> {
    return this.http.post<IComponentModel>(UrlStringsUtil.ComponentUrlEdit, {editedComponent, editIconData});
  }

  GetIndex(id: number): number{
    return this.flexItems === null ? -1 : this.flexItems.findIndex(item => item.id == id);
  }
}
