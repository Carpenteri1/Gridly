import {Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ComponentModel } from './Models/Component.Model';
import { Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class SharedService{
  flexItems: ComponentModel[] = [];
  isLoading = true;

  private apiUrl = 'http://localhost:7575/api/layout/'; // Replace with your actual API URL
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
    this.flexItems.push(newComponent);
      this.PostAddedComponentList(this.flexItems)
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

  PostAddedComponentList(componentList: ComponentModel[]): Observable<ComponentModel[]> {
    return this.http.post<ComponentModel[]>(this.apiUrl+"save", componentList,{
      responseType: 'json',
      headers: {'Content-Type': 'application/json'}
    }).pipe(
        //catchError(this.handleError('addHero', hero))
      );
  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.id == id);
  }
}
