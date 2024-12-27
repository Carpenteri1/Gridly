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

  urlPattern = /^(https?:\/\/)?(www\.)?(?!www\.)[A-Za-z]+(\.|\:)([a-zA-Z]{2,}|[0-9]+)$/;
  namePattern = /^[A-Za-z]+$/;
  wantToUploadIcon = false;
  wantToLinkToImage = false;
  IconFile:any = [];
  Name:string = "";
  Url:string = "";
  IconUrl:string = "";

  constructor(public sharedService: SharedService){}

  AddComponent() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.sharedService.GetId(newId);
    if(index === -1 && this.Name !== "" && this.Url !== ""){
      this.sharedService.AddComponent(new ComponentModel(newId,this.Name,this.Url));
    }
    else
      this.AddComponent();
  }
  get CanAddComponent(): boolean{
    if(this.Name !== "" && this.Url !== "" &&
      this.urlPattern.test(this.Url) && this.namePattern.test(this.Name)){
      if(this.IconUrl != null && this.IconUrl != "" ||
        this.IconFile != null && this.IconFile.length > 0){
        return true;
      }
    }
    return false;
  }

  onFileUpload(event:any):void{
    this.IconFile = event.target.files[0]
    debugger;
  }

  WantToUploadIcon(): void{
    this.wantToUploadIcon = true;
    this.wantToLinkToImage = false;
    console.log("upload image is now "+this.wantToUploadIcon);
  }

  WantToLinkToImage(): void{
    this.wantToLinkToImage = true;
    this.wantToUploadIcon = false;
    console.log("link to image is now "+this.wantToLinkToImage);
  }

}
