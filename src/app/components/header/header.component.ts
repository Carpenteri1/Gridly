import { Component } from "@angular/core";
import { SharedService } from '../../shared.service';
import { IComponentModel } from '../../interfaces/IComponent.Model'
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'header-component',
    standalone: true,
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
    imports: [FormsModule]
  })

export class HeaderComponent{
  IComponentModel: IComponentModel = {
    Id:0,
    Name:'test',
    Url:'localhost'
  }
  constructor(public sharedService: SharedService){}

  AddComponent(IComponentModel: IComponentModel) {
    this.sharedService.AddComponent(IComponentModel);
  }
}