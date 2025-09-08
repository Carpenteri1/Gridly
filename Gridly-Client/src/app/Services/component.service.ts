import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {MapComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {ComponentEndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {lastValueFrom} from "rxjs";
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

  get Components(): ComponentModel[] {
    return this.components;
  }

  set Components(components: ComponentModel[]) {
    this.components = components;
  }

  GetComponentById(id: number): ComponentModel | undefined{
    return this.Components.find((i: ComponentModel) => i.id === id);
  }

  set Component(item: ComponentModel) {
    if (item !== undefined) {
      this.component = MapComponentData(item);
    }
  }

  get Component() {
    return this.component;
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

  get InAnyMode(): boolean{
    return this.InEditMode || this.InResizeMode || this.InResizeMode;
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
  }

  async AddNewComponent(modalType: ModalViewModel) {
    let index = -1;
    if(this.Component !== undefined && this.Components.length > 0 && modalType.component !== undefined){
      index = this.componentEndpointService.GetIndex(modalType.component.id);
      modalType.component = MapComponentData.Override({id: Math.max(...this.components.map(x => x.id)) + 1}, modalType.component);
    }
    else if(this.components !== undefined){
      modalType.component = MapComponentData.Override({id:1},modalType.component);
    }

    if(index === -1 &&
      modalType.component?.name !== "" &&
      modalType.component?.url !== "")
      {
        if(modalType.component.iconData != null &&
          modalType.component.iconData !== undefined){
          if(modalType.component.iconData?.base64Data !== "" &&
            modalType.component.iconData?.name !== "" &&
            modalType.component.iconData?.type !== ""){
            await this.CallEndpoint(ComponentEndPointType.Add, modalType);
          }
        }
        if(modalType.component.iconUrl !== ""){
          await this.CallEndpoint(ComponentEndPointType.Add, modalType);
        }
      }
      else{
          await this.AddNewComponent(modalType);
      }
  }

  async EditComponentData(modalViewModel: ModalViewModel) {
    if(modalViewModel.component.id !== 0){
      await this.CallEndpoint(ComponentEndPointType.Edit,modalViewModel);
    }
  }

  async EditComponentsData(components: ComponentModel[]): Promise<void> {
    if(components !== undefined){
      await this.CallEndpoint(ComponentEndPointType.BatchEdit,undefined, undefined ,components);
    }
  }

  async CallEndpoint(type: ComponentEndPointType, modalViewModel?: ModalViewModel, componentData?: ComponentModel, componentsData?: ComponentModel[]): Promise<any> {
    switch(type){
      case ComponentEndPointType.Get:
        try {
          this.Components = await lastValueFrom(this.componentEndpointService.GetComponents());
          console.log(TextStringsUtil.GetComponentsSucceededEndPointSuccessMessage, this.Components);
        } catch (err) {
          console.error(TextStringsUtil.GetComponentsFailedEndPointMessage, err);
        }
        return this.Components;
      case ComponentEndPointType.Add:
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
          try {
            this.Component = await lastValueFrom(this.componentEndpointService.AddComponent(modalViewModel.component));
            console.log(TextStringsUtil.ComponentAddedEndPointSucceededMessage, this.Component);
          } catch (err) {
            console.error(TextStringsUtil.ComponentAddedFailedEndPointMessage, err);
          }
        }
        break;
      case ComponentEndPointType.Edit:
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
          try {
            this.Component =
              await lastValueFrom(this.componentEndpointService.EditComponent(
                modalViewModel.component,modalViewModel.selectedDropDownValue!));
            console.log(TextStringsUtil.ComponentEditSucceededEndPointMessage, this.Component);
          } catch (err) {
            console.error(TextStringsUtil.ComponentEditFailedEndPointMessage, err);
          }
        }
        break;
      case ComponentEndPointType.BatchEdit:
        if(componentsData !== undefined && componentsData !== null && componentsData.length > 0){
          try {
            this.Components = await lastValueFrom(this.componentEndpointService.EditComponents(componentsData));
            console.log(TextStringsUtil.ComponentBatchEditSucceededEndPointMessage, this.Component);
          } catch (err) {
            console.error(TextStringsUtil.ComponentBatchEditFailedEndPointMessage, err);
          }
        }
        break;
      case ComponentEndPointType.Delete:
        if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined){
          try {
            this.Component = await lastValueFrom(
              this.componentEndpointService.Delete(modalViewModel.component.id));
              console.log(TextStringsUtil.ComponentDeletedSucceededEndPointMessage, this.Component);
            } catch (err) {
              console.error(TextStringsUtil.ComponentDeletionFailedEndPointMessage, err);
            }
          }
          break
      case ComponentEndPointType.GetById:
        let component!: ComponentModel;

        if(componentData !== undefined && componentData !== null && componentsData)
          component = MapComponentData(componentData);
        else if(modalViewModel !== undefined && modalViewModel !== null && modalViewModel.component !== undefined)
          component = MapComponentData(modalViewModel.component);

        if(component !== undefined ) {
          try {
            this.Component = await lastValueFrom(this.componentEndpointService.GetComponentById(component.id));
            console.log(TextStringsUtil.ComponentGetByIdSucceededEndPointMessage, this.Component);
          } catch (err) {
            console.error(TextStringsUtil.ComponentGetByIdFailedEndPointMessage, err);
          }
        }
        return this.component;
      default:
        break;
    }
    this.ResetAllComponentData();
    this.DisableModes();
    window.location.reload();
  }

  async DeleteComponent(modalType: ModalViewModel) {
    await this.CallEndpoint(ComponentEndPointType.Delete, modalType);
  }

  public ResetAllComponentData(): void{
    this.Component = MapComponentData(undefined);
  }
}
