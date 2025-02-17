import {Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ComponentModel } from './Models/Component.Model';
import {catchError, Observable, throwError} from "rxjs";
import {IconModel} from "./Models/Icon.Model";

@Injectable({
  providedIn: 'root'
})

export class SharedService{
  flexItems: ComponentModel[] = [];
  isLoading = true;

  private componentUrl = '/api/component/';
  private versionUrl = '/api/version/';
  constructor(private http: HttpClient) {}
  RemoveComponent(id: number) {
    this.http.delete<ComponentModel[]>(`${this.componentUrl}delete/${id}`)
      .subscribe(() => {
          let index = this.GetId(id);
          if(index > -1){
            this.flexItems.splice(index, 1);
          }
      },
        (error) =>{
          console.error('Error deleting component:', error);
        });
  }

  EditComponent(editedComponent:ComponentModel, editedIconData?: IconModel) {
    this.PostEditComponentList(editedComponent, editedIconData)
      .subscribe()
    setTimeout(() => {
      this.LoadComponentList();
      this.ReloadPage();
    }, 500);
  }

  AddComponent(newComponent: ComponentModel) {
    this.PostAddedComponentList(newComponent)
      .subscribe()
    setTimeout(() => {
      this.LoadComponentList();
      this.ReloadPage();
    }, 500);
  }

  ReloadPage() {
    window.location.reload();
  }

  LoadComponentList() {
    if(this.flexItems === null ||
      this.flexItems.length === 0)
      this.isLoading = true;

    this.http.get<ComponentModel[]>(`${this.componentUrl}get`).subscribe(
      (components) => {
        this.flexItems = components;

        if(this.isLoading)
          this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching components:', error);
        this.isLoading = false;
      }
    );
  }

  CheckForNewRelease() {
    this.http.get<boolean>(`${this.versionUrl}latest`).subscribe(
      () => {
        this.ReloadPage();
      },
      error => {
        console.error('Error checking for new release:', error);
      }
    );
  }

  PostAddedComponentList(newComponent: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(this.componentUrl+"save", newComponent,{
      responseType: 'json',
      headers: {'Content-Type': 'application/json'}
    }).pipe(
      catchError(error => {
        console.error('Error posting save new component:', error);
        return throwError(error);
      }));
  };

  PostEditComponentList(editedComponent: ComponentModel, editedIconData?: IconModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(this.componentUrl+"edit",
      {
          editedComponent:editedComponent,
          editedIconData:editedIconData,
        },{
      responseType: 'json',
      headers: {'Content-Type': 'application/json'}
    }).pipe(
      catchError(error => {
        console.error('Error posting edit component:', error);
        return throwError(error);
      }));
  };

    GetId(id: number): number{
    return this.flexItems === null ? -1 : this.flexItems.findIndex(item => item.id == id);
  }
  GetComponentById(id: number): ComponentModel{
    let component = this.flexItems.find(item => item.id == id) as ComponentModel;
    if(component === undefined || component === null){
      if(this.flexItems === null ||
        this.flexItems.length === 0)
        this.isLoading = true;

      this.http.get<ComponentModel>(`${this.componentUrl}getbyid/${id}`).subscribe(
        (returnData) => {

          if(this.isLoading)
            this.isLoading = false;

          component = returnData;
        },
        (error) => {
          console.error('Error fetching component:', error);
          this.isLoading = false;
        }
      );
    }
    return component;
  }
}
