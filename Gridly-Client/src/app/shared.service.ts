import { Injectable } from '@angular/core';
import {ComponentModel} from "./Models/Component.Model";
import { HttpClient } from '@angular/common/http';

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
    this.flexItems.push(newComponent)
   // return this.http.post(`${this.apiUrl}/save`, this.flexItems);
    return this.http.post(`${this.apiUrl}/save`, this.flexItems);
  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.Id == id);
  }
}
