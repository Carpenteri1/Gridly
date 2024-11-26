import { Injectable } from '@angular/core';
import {ComponentModel} from "./Models/Component.Model";
import {Obj} from "@popperjs/core";

@Injectable({
  providedIn: 'root'
})

export class SharedService {
  flexItems: ComponentModel[] = [];

  Remove(id: number) {
    let index = this.GetId(id);
    if(index > -1){
      this.flexItems.splice(index, 1);
    }
  }

  AddComponent(newComponent: ComponentModel) {
    this.flexItems.push(newComponent)
  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.Id == id);
  }
}
