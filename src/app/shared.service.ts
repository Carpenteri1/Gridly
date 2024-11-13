import { Injectable } from '@angular/core';
import { IComponentModel } from './interfaces/IComponent.Model';

@Injectable({
  providedIn: 'root'
})

export class SharedService {
  flexItems: { IComponentModel: IComponentModel }[] = [];

  Remove(id: number) {
    let index = this.GetId(id);
    if(index > -1){
      this.flexItems.splice(index, 1);
    }
  }

  AddComponent(IComponentModel: IComponentModel) {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.GetId(newId);
    if(index === -1){
      IComponentModel.Id = newId;
      this.flexItems.push({ IComponentModel: IComponentModel });
    }
    else
      this.AddComponent(IComponentModel);
  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.IComponentModel.Id == id);
  }
}