import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ComponentModel } from './Models/Component.Model';

@Injectable({
  providedIn: 'root'
})

export class SharedService {
  flexItems: ComponentModel[] = [];
  private apiUrl = 'http://localhost:7575/api/layout/save'; // Replace with your actual API URL
  constructor(private http: HttpClient) {}

  Remove(id: number) {
    let index = this.GetId(id);
    if(index > -1){
      this.flexItems.splice(index, 1);
    }
  }

  AddComponent(newComponent: ComponentModel) {
    this.http.post<ComponentModel>(this.apiUrl, newComponent, {
      responseType: 'json',
      headers: {'Content-Type': 'application/json'}
    }).subscribe(
      Response => {
        this.flexItems.push(newComponent);
      },
      error => {
        console.error('There was an error!', error);
      }
    )

  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.Id == id);
  }
}
