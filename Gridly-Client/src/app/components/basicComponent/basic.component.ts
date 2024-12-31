import {Component, OnInit} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'basic-component',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './basic.component.html',
  styleUrls: ['./basic.component.css'],
})
export class BasicComponent implements OnInit {
  constructor(public sharedService: SharedService) {}

  ngOnInit() {
    this.sharedService.LoadComponentList();
  }

  HaveIconSet(name:string | undefined):boolean{
    return name !== undefined && name !== "";
  }
  HaveImagUrlSet(imageUrl:string | undefined):boolean{
    return imageUrl !== undefined && imageUrl !== "";
  }

  IconFilePath(name:string | undefined, fileType:string | undefined): string {
    debugger;
    if(name !== undefined && fileType !== undefined) {
      return "Assets/Icons/" + name + "." + fileType;
    }
    return "";
  }

  Remove(id: number): void {
    this.sharedService.RemoveComponent(id);
  }
}
