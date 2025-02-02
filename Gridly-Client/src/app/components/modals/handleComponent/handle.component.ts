import {Component, Injectable, Input} from "@angular/core";
import {IconModel} from "../../../Models/Icon.Model";
import {ComponentModel} from "../../../Models/Component.Model";
import {SharedService} from "../../../shared.service";
import {FormsModule} from "@angular/forms";

@Component({
  selector: 'handle-component-modal',
  standalone: true,
  templateUrl: './handle.component.html',
  imports: [
    FormsModule
  ],
})

@Injectable({ providedIn: 'root' })
export class HandleComponent {
  urlPattern = /^(https?:\/\/)(www\.)?(?!www\.)[A-Za-z0-9.]+(\.|\:)[a-zA-Z0-9]{2,}$/;
  imageUrlPattern = /^(https?:\/\/)(www\.)?(?!www\.)[A-Za-z0-9-.]+(\.|\:)[a-zA-Z0-9?=:\/&]{2,}$|^(data:image\/)(png|svg|jpeg|jpg|ico)(;base64,).*/;
  namePattern = /^[A-Za-z]+$/;

  @Input() btnIcon: string ="";
  @Input() btnTheme: string ="";
  @Input() btnTitle: string ="";
  @Input() windowTitle: string ="";
  @Input() modalLabelId: string = "";
  @Input() modalBindDropdownId: string = "";
  @Input() editMode: boolean = false;

  wantToUploadIcon = false;
  wantToLinkToImage = false;

  iconData= new IconModel("","","");
  @Input() component = new ComponentModel(0,"","", this.iconData,"");
  @Input() acceptButton!: () => void;

  triggerAccept() {
    if (this.acceptButton) {
      this.acceptButton();
    }
  }
  constructor(public sharedService: SharedService){
  }

  public AddComponent() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.sharedService.GetId(newId);

    if(index === -1 && this.component.name !== "" && this.component.url !== "" )
    {
      if(this.iconData.base64Data !== "" && this.iconData.name !== "" && this.iconData.fileType !== ""){
        this.sharedService.AddComponent(new ComponentModel(newId, this.component.name, this.component.url, this.iconData, undefined));
      }
      if(this.component.imageUrl !== ""){
        this.sharedService.AddComponent(new ComponentModel(newId, this.component.name, this.component.url,undefined, this.component.imageUrl));
      }
      this.ResetFormData();
    }
    else
      this.AddComponent();
  }

  public EditComponent(){
        this.sharedService.EditComponent(this.component, this.iconData);
  }

  public CanEditComponent() {
    return
      this.editMode &&
      this.NoEmptyInputFields &&
      JSON.stringify(this.sharedService.GetComponentById) !== JSON.stringify(this.component);
  }

  get CanAddComponent() :boolean{
    return this.NoEmptyInputFields;
  }

  get NoEmptyInputFields(): boolean{
    if(this.component.name !== "" && this.component.url !== "" &&
      this.urlPattern.test(this.component.url) && this.namePattern.test(this.component.name) ){

      if(this.iconData.name !== "" || this.component.imageUrl !== "" && this.component.imageUrl !== undefined &&
        this.imageUrlPattern.test(this.component.imageUrl)){
        return true;
      }
    }
    return false;
  }

  ResetFormData(): void{
    this.component.id = 0;
    this.component.name = "";
    this.component.url = "";
    this.ResetIconData();
  }

  ResetIconData(){
    this.iconData.base64Data = "";
    this.iconData.name = "";
    this.iconData.fileType = "";
    this.component.imageUrl = "";
  }

  OnFileUpload(event:any):void{
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      this.SetFileNameAndType(file.name);

      if(this.iconData.fileType === 'svg' ||
        this.iconData.fileType === 'png' ||
        this.iconData.fileType === 'jpg' ||
        this.iconData.fileType === 'jpeg' ||
        this.iconData.fileType === 'ico')
      {
        const reader = new FileReader();
        reader.onload = () => {
          this.iconData.base64Data = reader.result as string;
          this.iconData.base64Data =  this.iconData.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
      }
    }
  }

  private SetFileNameAndType(inputName: string){
    let result = inputName.split('.');
    this.iconData.name = result[0];
    this.iconData.fileType = result[1];
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
