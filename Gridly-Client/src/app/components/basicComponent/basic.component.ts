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
    return name !== undefined && name != null && name !== "";
  }
  HaveImagUrlSet(imageUrl:string | undefined):boolean{
    return imageUrl !== undefined && imageUrl != null && imageUrl !== "";
  }

  IconFilePath(name:string | undefined, fileType:string | undefined): string {
    return "Assets/Icons/" + name + "." + fileType;
  }

  Remove(id: number): void {
    this.sharedService.RemoveComponent(id);
  }
}
