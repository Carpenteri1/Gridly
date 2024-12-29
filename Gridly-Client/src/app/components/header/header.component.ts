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
  iconData: string = '';
  name:string = '';
  url:string = '';

  constructor(public sharedService: SharedService){}

  AddComponent() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.sharedService.GetId(newId);
    if(index === -1 && this.name !== "" && this.url !== "" && this.iconData !== ""){
      this.sharedService.AddComponent(new ComponentModel(
        newId,this.name,this.url,this.iconData));
      this.ResetFormData();
    }
    else
      this.AddComponent();
  }
  get CanAddComponent(): boolean{
    if(this.name !== "" && this.url !== "" &&
      this.urlPattern.test(this.url) && this.namePattern.test(this.name) ){

      if(this.iconData !== null && this.iconData !== ""){
        return true;
      }
    }
    return false;
  }

  ResetFormData(): void{
    this.name = '';
    this.url = '';
    this.iconData = '';
  }

  ResetIconData(){
    this.iconData = '';
  }

  OnFileUpload(event:any):void{
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.iconData = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  WantToUploadIcon(): void{
    this.wantToUploadIcon = true;
    this.wantToLinkToImage = false;
    this.ResetIconData();
  }

  WantToLinkToImage(): void{
    this.wantToLinkToImage = true;
    this.wantToUploadIcon = false;
    this.ResetIconData();
  }

}
