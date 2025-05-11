import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { IconModel } from "../../../Models/Icon.Model";
import { ComponentModel } from "../../../Models/Component.Model";
import { SharedService } from '../../../Services/shared.service';
import { FormsModule } from "@angular/forms";
import { RegexUtil } from "../../../Utils/regex.util";

@Component({
    selector: 'handle-component-modal',
    templateUrl: './handle.component.html',
    styleUrl: './handle.component.css',
    standalone: true,
    imports: [FormsModule]
})

export class HandleComponent implements AfterViewInit, OnInit {

  @Input() btnIcon!:string;
  @Input() btnTheme!: string;
  @Input() btnTitle!: string;
  @Input() windowTitle!: string;
  @Input() modalLabelId!: string;
  @Input() modalBindDropdownId!: string;
  @Input() editMode!: boolean;
  wantToUploadIcon!: boolean;
  wantToLinkToImage!: boolean;

  iconData!: IconModel;
  @Input() component!: ComponentModel;
  @Input() acceptButton!: () => void;
  @ViewChild('modalElement') modalRef!: ElementRef;

  constructor(public sharedService: SharedService){}

  ngAfterViewInit() {
    //this.listenHideModal();//TODO bug fix
  }

  ngOnInit() {
    this.wantToUploadIcon = false;
    this.wantToLinkToImage = false;
  }

  triggerAccept() {
    if (this.acceptButton) {
      this.acceptButton();
    }
  }

  public AddComponent() {
    const newId = Math.floor(Math.random() * 100) + 1;
    let index = this.sharedService.GetIndex(newId);

    if(index === -1 && this.component.name !== "" && this.component.url !== "" )
    {
      if(this.iconData.base64Data !== "" && this.iconData.name !== "" && this.iconData.fileType !== ""){
        this.sharedService.AddComponent(new ComponentModel(newId, this.component.name, this.component.url, this.iconData, undefined, false,false));
      }
      if(this.component.imageUrl !== ""){
        this.sharedService.AddComponent(new ComponentModel(newId, this.component.name, this.component.url,undefined, this.component.imageUrl, false, false,));
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
      RegexUtil.urlPattern.test(this.component.url) && RegexUtil.namePattern.test(this.component.name) ){

      if((this.iconData.name !== "" || this.editMode) || (this.component.imageUrl !== "" &&
        this.component.imageUrl !== undefined &&
        RegexUtil.imageUrlPattern.test(this.component.imageUrl))){
        return true;
      }
    }
    return false;
  }

  ResetFormData(): void{
    this.component.id = 0;
    this.component.name = "";
    this.component.url = "";
    this.component.titleHidden = false;
    this.component.imageHidden = false;
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

  HideTitle(event: Event) {
    this.component.titleHidden = (event.target as HTMLInputElement).checked;
  }

  HideImage(event: Event) {
    this.component.imageHidden = (event.target as HTMLInputElement).checked;
  }

  /*listenHideModal() {
    this.modalRef.nativeElement.addEventListener('blur', () => {
      this.ResetFormData();
    });
  }*/
}
