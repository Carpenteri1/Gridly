import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {MapComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {EndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {take} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService{
  private editMode!: boolean;
  private dragMode!: boolean;
  private resizeModeActive!: boolean;
  private iconHidden!: boolean;
  private iconUrlHidden!: boolean;
  private components!: ComponentModel[];
  private component!: ComponentModel;

  constructor(public componentEndpointService: ComponentEndpointService) {
    this.editMode = false;
    this.dragMode = false;
    this.resizeModeActive = false;
  }

  get GetAllComponents(): ComponentModel[] {
    return this.components;
  }

  SelectedComponent(item: ComponentModel) : ComponentModel {
    if(this.GetComponentById(item) !== undefined){
      return this.component = MapComponentData(item);
    }
    return this.component;
  }

  GetComponentById(item: ComponentModel): ComponentModel | undefined{
    return this.GetAllComponents.find((i: ComponentModel) => i.id === item.id);
  }

  IconDataSet(item :ComponentModel) {
    return item.iconData != undefined &&
      item.iconData.name !== "" &&
      item.iconData.type !== undefined &&
      item.iconData.base64Data !== "" ;
      //&&
      //!this.component.imageHidden;
  }

  IconUrlSet(item :ComponentModel): boolean {
    return  item.iconUrl !== undefined  &&
      item.iconUrl !== ""
      //&&
      //RegexStringsUtil.iconUrlPattern.test(item.iconUrl);
      //&&
     // !this.component.imageHidden;
  }

  get IconIsUrlHidden(){
    return this.iconUrlHidden;
  }

  get IconIsFileHidden(){
    return this.iconHidden;
  }

  get ResizeModeActive(): boolean {
    return this.resizeModeActive;
  }

  CheckComponentData(item:ComponentModel): boolean {
    return item !== undefined &&
      item.name !== "" &&
      item.url !== "" &&
      RegexStringsUtil.urlPattern.test(item.url) &&
      RegexStringsUtil.namePattern.test(item.name);
  }

  get InEditMode(): boolean {
    return this.editMode;
  }

  get InResizeMode(): boolean {
    return this.resizeModeActive;
  }

  get InDragMode(): boolean {
    return this.dragMode;
  }

  SwitchEditMode(): void {
    this.editMode = !this.editMode;
    this.resizeModeActive = false;
    this.dragMode = false;
  }

  SwitchResizeMode(): void {
    this.resizeModeActive = !this.resizeModeActive;
    this.dragMode = false;
    this.editMode = false;
  }

  SwitchDragMode(): void {
    this.dragMode = !this.dragMode;
    this.editMode = false;
    this.resizeModeActive = false;
  }

  DisableModes(){
    this.editMode = false;
    this.resizeModeActive = false;
    this.dragMode = false;
    //window.location.reload();
  }

  AddNewComponent(component: ComponentModel) {
    let index = -1;
    if(this.components !== undefined && this.components.length > 0 && component !== undefined){
      index = this.componentEndpointService.GetIndex(component.id);
      component = MapComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});
    }
    else if(this.components !== undefined){
      component = MapComponentData(component,{id:1});
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
        if(component.iconUrl !== ""){
          this.CallEndpoint(EndPointType.Add, component);
        }
      }
      else{
          this.AddNewComponent(component);
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

  CallEndpoint(type: EndPointType, componentData?: ComponentModel, componentsData?: ComponentModel[]): any {
    switch(type){
      case EndPointType.Get:
        this.componentEndpointService.GetComponents().pipe(take(1)).subscribe({
          next: (data) => {
            console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, data);
            this.components = data},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)});
        return this.components;
      case EndPointType.Add:
        if(componentData !== undefined && componentData !== null){
          this.componentEndpointService.AddComponent(MapComponentData(componentData)).pipe(take(1)).subscribe({
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
          break
      case EndPointType.GetById:
        if(componentData !== undefined && componentData !== null){
          this.componentEndpointService.GetComponentById(componentData.id).pipe(take(1)).subscribe({
            next: (res) =>{console.log(TextStringsUtil.ComponentDeletedSuccessEndPointMessage, res);
              this.component = res},
            error: (err) => console.error(TextStringsUtil.ComponentDeletionFailedEndPointMessage, err)
          });
        }
        return this.component;
      default:
        break;
    }
  }

  DeleteComponent(componentData: ComponentModel) {
    this.CallEndpoint(EndPointType.Delete, componentData)
  }

  public ResetAllComponentData(): void{
    this.component.id = 0;
    this.component.name = "";
    this.component.url = "";
    this.component.titleHidden = false;
    this.component.imageHidden = false
    this.ResetComponentImageUrl();
    this.ResetComponentIconData();
  }

  public ResetComponentImageUrl(){
    this.component.iconUrl = "";
  }

  public ResetComponentIconData(){
    this.component.iconData = undefined;
  }

}
