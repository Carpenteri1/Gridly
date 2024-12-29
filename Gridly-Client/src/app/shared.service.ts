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

  AddComponent(newComponent: ComponentModel) {
    if (!this.flexItems) {
      this.flexItems = [];
    }
    this.flexItems.push(newComponent);
    this.PostAddedComponentList(newComponent)
      .subscribe();
  }
  LoadComponentList() {
    this.isLoading = true;
    this.http.get<ComponentModel[]>(`${this.apiUrl}get`).subscribe(
      (components) => {
        this.flexItems = components; // Update the flexItems array with the fetched components
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
