import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {SetComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {EndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";
import { take } from "rxjs";

@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  components!: ComponentModel[];
  component!: ComponentModel;

  constructor(public componentEndpointService: ComponentEndpointService) {
    this.editMode = false;
    this.dragMode = false;
    this.resizeMode = false;
  }

  SwitchEditMode(): void {
    this.editMode = !this.editMode;
    this.resizeMode = false;
    this.dragMode = false;
  }

  SwitchResizeMode(): void {
    this.resizeMode = !this.resizeMode;
    this.dragMode = false;
    this.editMode = false;
  }

  SwitchDragMode(): void {
    this.dragMode = !this.dragMode;
    this.editMode = false;
    this.resizeMode = false;
  }

  DisableModes(){
    this.editMode = false;
    this.resizeMode = false;
    this.dragMode = false;
    //window.location.reload();
  }

  AddComponent(component: ComponentModel) {
    let index = -1;
    if(this.components !== undefined && this.components.length > 0 && component !== undefined){
      index = this.componentEndpointService.GetIndex(component.id);
      component = SetComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});
    }
    else if(this.components !== undefined){
      component = SetComponentData(component,{id:1});
    }

    if(index === -1 &&
        component.name !== "" &&
        component.url !== "")
      {
        if(component.iconData != null && component.iconData !== undefined){
          if(component.iconData?.base64Data !== "" &&
            component.iconData?.name !== "" &&
            component.iconData?.type !== ""){
            this.CallEndpoint(EndPointType.Add, component);
          }
        }
        if(component.imageUrl !== ""){
          this.CallEndpoint(EndPointType.Add, component);
        }
      }
      else{
          this.AddComponent(component);
      }
  }

  EditComponentData(component: ComponentModel) {
    if(component.id !== 0){
      this.CallEndpoint(EndPointType.Edit, component);
      this.DisableModes();
    }
  }

  EditComponentsData(components: ComponentModel[]) {
    if(components !== undefined){
      this.CallEndpoint(EndPointType.BatchEdit,undefined ,components);
      this.DisableModes();
      //window.location.reload();
    }
  }

  CallEndpoint(type: EndPointType, componentData?: ComponentModel, componentsData?: ComponentModel[]) {
    switch(type){
      case EndPointType.Get:
        this.componentEndpointService.GetComponents().pipe(take(1)).subscribe({
          next: (data) => {
            console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, data);
            this.components = data},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)});
        break;
      case EndPointType.Add:
        if(componentData !== undefined && componentData !== null){
          this.componentEndpointService.AddComponent(SetComponentData(componentData)).pipe(take(1)).subscribe({
            next: (res) => {console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)
          });
        }
        break;
      case EndPointType.Edit:
        if(componentData !== undefined && componentData !== null){
        this.componentEndpointService.EditComponent(componentData).pipe(take(1)).subscribe({
          next: (res) =>{console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, res)},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)});
        }
        break;
      case EndPointType.BatchEdit:
        if(componentsData !== undefined && componentsData !== null && componentsData.length > 0){
          this.componentEndpointService.EditComponents(componentsData).pipe(take(1)).subscribe({
            next: (res) =>{console.log(TextStringsUtil.ComponentsSavedEndPointSuccessMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentsFailedEndPointSuccessMessage, err)
          });
        }
        break;
      case EndPointType.Delete:
        if(componentData !== undefined && componentData !== null){
          this.componentEndpointService.Delete(componentData.id).pipe(take(1)).subscribe({
            next: (res) =>{console.log(TextStringsUtil.ComponentDeletedSuccessEndPointMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentDeletionFailedEndPointMessage, err)
          });
        }
          break;
      default:
        break;
    }
  }

  DeleteComponent(componentData: ComponentModel) {
    this.CallEndpoint(EndPointType.Delete, componentData)
  }
}
