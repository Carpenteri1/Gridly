import {Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ComponentModel } from './Models/Component.Model';
import {catchError, Observable, throwError} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class SharedService{
  flexItems: ComponentModel[] = [];
  isLoading = true;

  private apiUrl = '/api/layout/';
  constructor(private http: HttpClient) {}
  RemoveComponent(id: number) {
    this.http.delete<ComponentModel[]>(`${this.apiUrl}delete/${id}`)
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

  EditComponent(component:ComponentModel){
    this.RemoveComponent(component.id);
    this.AddComponent(component);
  }

  AddComponent(newComponent: ComponentModel) {
    this.PostAddedComponentList(newComponent)
      .subscribe()
    setTimeout(() => {
      this.LoadComponentList();
    }, 500);
  }
  LoadComponentList() {
    if(this.flexItems === null ||
      this.flexItems.length === 0)
      this.isLoading = true;

    this.http.get<ComponentModel[]>(`${this.apiUrl}get`).subscribe(
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

  PostAddedComponentList(newComponent: ComponentModel): Observable<ComponentModel> {
    return this.http.post<ComponentModel>(this.apiUrl+"save", newComponent,{
      responseType: 'json',
      headers: {'Content-Type': 'application/json'}
    }).pipe(
      catchError(error => {
        console.error('Error posting save new component:', error);
        return throwError(error);
      }));
  };

    GetId(id: number): number{
    return this.flexItems === null ? -1 : this.flexItems.findIndex(item => item.id == id);
  }
}
