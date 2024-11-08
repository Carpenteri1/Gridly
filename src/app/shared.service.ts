import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class SharedService {
  flexItems: { id: number }[] = [];

  Remove(id: number) {
    let index = this.GetId(id);
    if(index > -1){
      this.flexItems.splice(index, 1);
    }
  }

  AddItem() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.GetId(newId);
    console.log(index);
    if(index === -1){
      this.flexItems.push({ id: newId });
    }
    else
      this.AddItem();
  }

  GetId(id: number): number{
    return this.flexItems.findIndex(item => item.id == id);
  }
}