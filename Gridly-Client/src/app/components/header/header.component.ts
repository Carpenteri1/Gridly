import { Component } from "@angular/core";
import { SharedService } from '../../shared.service';
import { ComponentModel } from '../../Models/Component.Model'
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'header-component',
    standalone: true,
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
    imports: [FormsModule]
  })

export class HeaderComponent{
  constructor(public sharedService: SharedService){}
  Name:string = "";
  Url:string = "";
  AddComponent() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.sharedService.GetId(newId);
    if(index === -1){
      this.sharedService.AddComponent(new ComponentModel(newId,this.Name,this.Url));
    }
    else
      this.AddComponent();
  }
}
