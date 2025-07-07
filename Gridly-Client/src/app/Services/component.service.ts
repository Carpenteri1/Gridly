import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {SetComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {take} from "rxjs";
import {IconModel} from "../Models/Icon.Model";
import {EndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  components!: ComponentModel[];
  component!: ComponentModel;
  iconData!: IconModel;

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
  }

  AddComponent(component: ComponentModel): boolean {
      component = SetComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});
      let index = this.componentEndpointService.GetIndex(component.id);
      if(index === -1 &&
        component.name !== "" &&
        component.url !== "")
      {
        if(component.iconData != null || component.iconData !== undefined){
          if(component.iconData?.base64Data !== "" &&
            component.iconData?.name !== "" &&
            component.iconData?.type !== ""){
            this.CallEndpoint(EndPointType.Add, component);
            return true;
          }
          if(component.imageUrl !== ""){
            this.CallEndpoint(EndPointType.Add, component);
            return true;
          }
        }
      }
      else{
        this.AddComponent(component);
      }
      return false;
  }

  EditComponentData(component: ComponentModel) : boolean {
    if(component.id !== 0){
      this.CallEndpoint(EndPointType.Edit, component);
      this.DisableModes();
      return true;
    }
    return false;
  }

  EditComponentsData(components: ComponentModel[]) : boolean {
    if(components !== undefined){
      this.CallEndpoint(EndPointType.BatchEdit,undefined ,components);
      this.DisableModes();
      return true;
    }
    return false;
  }

  CallEndpoint(type: EndPointType, componentData?: ComponentModel, componentsData?: ComponentModel[]) {
    let callStatus = false;
    switch(type){
      case EndPointType.Get:
        this.componentEndpointService.GetComponents().pipe(take(1)).subscribe({
          next: (data) => {
            callStatus = true;
            console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, data);
            this.components = data},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)});
        return callStatus;
      case EndPointType.Add:
        if(componentData != undefined) {
          this.componentEndpointService.AddComponent(SetComponentData(componentData)).pipe(take(1)).subscribe({
            next: (res) => { callStatus = true; console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)
          });
        }
        else{
          this.HandleInvalidData();
        }
        return callStatus;
      case EndPointType.Edit:
        if(componentData !== undefined) {
        this.componentEndpointService.EditComponent(componentData).pipe(take(1)).subscribe({
          next: (res) =>{callStatus = true; console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, res)},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)
        });
        }
        else{
          this.HandleInvalidData();
        }
        return callStatus;
      case EndPointType.BatchEdit:
        if(componentsData !== undefined) {
          this.componentEndpointService.EditComponents(componentsData).pipe(take(1)).subscribe({
            next: (res) =>{callStatus = true; console.log(TextStringsUtil.ComponentsSavedEndPointSuccessMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentsFailedEndPointSuccessMessage, err)
          });
        }
        else{
          this.HandleInvalidData();
        }
        return callStatus;
      case EndPointType.Delete:
        if(componentData != undefined) {
          this.componentEndpointService.Delete(componentData.id).pipe(take(1)).subscribe({
            next: (res) =>{callStatus = true; console.log(TextStringsUtil.ComponentDeletedSuccessEndPointMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentDeletionFailedEndPointMessage, err)
          });
        }
        else{
          this.HandleInvalidData();
        }
        return callStatus;
      default:
        this.componentEndpointService.Throw400Error().pipe(take(1)).subscribe({next: () =>{callStatus = false;}});
        return callStatus;
    }
  }

  private HandleInvalidData() {
    return this.componentEndpointService.Throw404Error().pipe(take(1)).subscribe({next: () =>{ let callStatus = false;}});
  }
  DeleteComponent(componentData: ComponentModel) {F
    this.CallEndpoint(EndPointType.Delete, componentData)
    return true;
  }
}
