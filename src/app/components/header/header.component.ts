import { Component } from "@angular/core";
import { SharedService } from '../../shared.service';

@Component({
    selector: 'header-component',
    standalone: true,
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
  })

export class HeaderComponent{

  constructor(public sharedService: SharedService) {}

  addComponent() {
    this.sharedService.addItem();
  }
  removeComponent() {
    this.sharedService.removeItem();
  }
}