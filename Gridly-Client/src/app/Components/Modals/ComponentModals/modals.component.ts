import {Component} from "@angular/core";
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {IModalsModel} from "../../../Models/IModalsModel";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {ComponentModel} from "../../../Models/Component.Model";
import {FormsModule} from "@angular/forms";
import {FormType} from "../../../Types/form.types.enum";
import {ComponentEndpointService} from "../../../Services/endpoints/component.endpoint.service";
import {IconModel} from "../../../Models/Icon.Model";
import {MatButton} from "@angular/material/button";
import {MatDivider} from "@angular/material/divider";

@Component({
  selector: 'modals-component',
  templateUrl: './modals.component.html',
  standalone: true,
  imports: [MatDialogClose, MatDialogContent, MatDialogActions, MatFormField, MatLabel, MatFormField, FormsModule, MatInput, MatDialogTitle, MatButton, MatDivider]
})
export class ModalsComponent {
  public viewModel!: IModalsModel;
  public components!: ComponentModel[];
  public iconData!: IconModel

  constructor(public endpointService: ComponentEndpointService){}

  Submit(type: FormType): boolean {
    console.log(this.viewModel.Type)
    switch (type) {
      case FormType.Add:
        return this.AddComponent();
      /*case FormType.Edit:
        //TODO maybe do something
        break;
      case FormType.Drag:
        //TODO maybe do something
        break;
      case FormType.Resize:
        //TODO maybe do something
        break;*/
      default:
          return false;
    }
  }

  private AddComponent(newId?: number) : boolean {
    this.endpointService.GetComponents()
      .subscribe((components) => {
        this.components = components.sort((a, b) => a.id - b.id);
      });

    newId = this.components[0].id ++;
    let index = this.endpointService.GetIndex(newId);
      if(index === -1 && this.viewModel.Component.Name !== "" && this.viewModel.Component.Url !== "")
    {
      if(this.iconData.base64Data !== "" && this.iconData.name !== "" && this.iconData.fileType !== ""){
        this.endpointService.AddComponent(new ComponentModel(newId, this.viewModel.Component.Name, this.viewModel.Component.Url, undefined, false,false, this.iconData));
      }
      if(this.viewModel.Component.ImageUrl !== ""){
        this.endpointService.AddComponent(new ComponentModel(newId, this.viewModel.Component.Name, this.viewModel.Component.Url, this.viewModel.Component.ImageUrl, false, false,));
      }
      this.ResetFormData();
      return true;
    }
    else
      this.AddComponent(newId++);

    return false;
  }

  private ResetFormData(): void{
    this.viewModel.Component.Id = 0;
    this.viewModel.Component.Name = "";
    this.viewModel.Component.Url = "";
    this.viewModel.Component.TitleHidden = false;
    this.viewModel.Component.ImageHidden = false;
    this.ResetIconData();
  }

  private ResetIconData(){
    this.iconData.base64Data = "";
    this.iconData.name = "";
    this.iconData.fileType = "";
    this.viewModel.Component.ImageUrl = "";
  }
}
