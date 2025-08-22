import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {MapComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {EndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {take} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";
import {ModalViewModel} from "../Models/ModalView.Model";

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

  AddNewComponent(modalType: ModalViewModel) {
    let index = -1;
    if(this.components !== undefined && this.components.length > 0 && modalType.component !== undefined){
      index = this.componentEndpointService.GetIndex(modalType.component.id);
      modalType.component = MapComponentData(modalType.component,{id: Math.max(...this.components.map(x => x.id)) + 1});
    }
    else if(this.components !== undefined){
      modalType.component = MapComponentData(modalType.component,{id:1});
    }

    if(index === -1 &&
      modalType.component?.name !== "" &&
      modalType.component?.url !== "")
      {
        if(modalType.component.iconData != null && modalType.component.iconData !== undefined){
          if(modalType.component.iconData?.base64Data !== "" &&
            modalType.component.iconData?.name !== "" &&
            modalType.component.iconData?.type !== ""){
            this.CallEndpoint(EndPointType.Add, modalType);
          }
        }
        if(modalType.component.iconUrl !== ""){
          this.CallEndpoint(EndPointType.Add, modalType);
        }
      }
      else{
          this.AddNewComponent(modalType);
      }
  }

  EditComponentData(modalViewModel: ModalViewModel) {
    if(modalViewModel.component.id !== 0){
      this.CallEndpoint(EndPointType.Edit,modalViewModel);
      this.DisableModes();
    }
  }

  EditComponentsData(components: ComponentModel[]) {
    if(components !== undefined){
      this.CallEndpoint(EndPointType.BatchEdit,undefined, undefined ,components);
      this.DisableModes();
      //window.location.reload();
    }
  }

  CallEndpoint(type: EndPointType, modalViewModel?: ModalViewModel, componentData?: ComponentModel, componentsData?: ComponentModel[]): any {
    switch(type){
      case EndPointType.Get:
        this.componentEndpointService.GetComponents().pipe(take(1)).subscribe({
          next: (data) => {
            console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, data);
            this.components = data},
          error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)});
        return this.components;
      case EndPointType.Add:
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
          this.componentEndpointService.AddComponent(MapComponentData(modalViewModel.component)).pipe(take(1)).subscribe({
            next: (res) => {console.log(TextStringsUtil.ComponentSavedEndPointSuccessMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentSavedFailedEndPointMessage, err)
          });
        }
        break;
      case EndPointType.Edit:
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
        this.componentEndpointService.EditComponent(modalViewModel.component, modalViewModel.selectedDropDownValue!).pipe(take(1)).subscribe({
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
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
          this.componentEndpointService.Delete(modalViewModel.component.id).pipe(take(1)).subscribe({
            next: (res) =>{console.log(TextStringsUtil.ComponentDeletedSuccessEndPointMessage, res)},
            error: (err) => console.error(TextStringsUtil.ComponentDeletionFailedEndPointMessage, err)
          });
        }
          break
      case EndPointType.GetById:
        let component!: ComponentModel;

        if(componentData !== undefined && componentData !== null && componentsData)
          component = MapComponentData(componentData);
        else if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined)
          component = MapComponentData(modalViewModel.component);

        if(component !== undefined ) {
          this.componentEndpointService.GetComponentById(component.id).pipe(take(1)).subscribe({
            next: (res) =>{console.log(TextStringsUtil.ComponentGetByIdSuccessEndPointMessage, res);
              this.component = res},
            error: (err) => console.error(TextStringsUtil.ComponentGetByIdFailedEndPointMessage, err)
          });
        }
        return this.component;
      default:
        break;
    }
  }

  DeleteComponent(modalType: ModalViewModel) {
    this.CallEndpoint(EndPointType.Delete, modalType);
  }

  public ResetAllComponentData(): void{
    this.component.id = 0;
    this.component.name = "";
    this.component.url = "";
    this.component.titleHidden = false;
    this.component.imageHidden = false;
    this.component.iconData = undefined;
    this.component.iconUrl = "";
  }
}
