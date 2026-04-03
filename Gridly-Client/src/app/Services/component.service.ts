import {Injectable, signal} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {ComponentEndPointType} from "../Types/endPoint.type.enum";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {lastValueFrom} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";
import {ModalViewModel} from "../Models/ModalView.Model";

@Injectable({providedIn: 'root'})
export class ComponentService{
  private editMode!: boolean;
  private iconHidden!: boolean;
  private iconUrlHidden!: boolean;
  private openEdit!: boolean;
  private modalId!:number;
  private components!: ComponentModel[];
  private component!: ComponentModel;

  constructor(public componentEndpointService: ComponentEndpointService) {
    this.editMode = false;
  }

  get Components(): ComponentModel[] {
    return this.components;
  }

  get Component() {
    return this.component;
  }

  set Components(components: ComponentModel[]) {
    this.components = components;
  }

  set Component(item: ComponentModel) {
    if (item !== undefined) {
      this.component = item;
    }
  }

  GetComponentById(id: number): ComponentModel | undefined{
    return this.Components.find((i: ComponentModel) => i.id === id);
  }

  IconDataSet(item :ComponentModel) {
    return item.iconData != undefined &&
      item.iconData.name !== "" &&
      item.iconData.type !== undefined &&
      item.iconData.base64Data !== "" &&
      !item.componentSettings?.imageHidden;
  }

  IconUrlSet(item :ComponentModel): boolean {
    return  item.iconUrl !== undefined  &&
      item.iconUrl !== "" &&
      RegexStringsUtil.iconUrlPattern.test(item.iconUrl) &&
      !item.componentSettings?.imageHidden;
  }

    MaterialIconSet(item :ComponentModel): boolean {
    return  item.iconData?.materialIcon !== undefined  &&
      item.iconData?.materialIcon !== "" &&
      !item.componentSettings?.imageHidden;
  }

  get ComponentHasBaseData(){
    return this.component.name !== undefined && "" &&
      this.component.url !== undefined && "" &&
      this.component.iconData !== undefined ||
      this.component.iconUrl !== undefined && "";
  }

  get IconIsUrlHidden(){
    return this.iconUrlHidden;
  }

  get IconIsFileHidden(){
    return this.iconHidden;
  }

  get ResizeModeActive(): boolean {
    return this.editMode;
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

  ToggleEditMode(): void {
    this.editMode = !this.editMode;
    if(!this.editMode)
      window.location.reload();
  }

  ToggleOpenEditWidget(id:number): void {
    this.ModalId = id;
    this.openEdit = !this.openEdit;
  }

  OpenEditWidget(id:number): void {
    this.ModalId = id;
    this.openEdit = true;
  }

  get OpenEdit(){
    return this.openEdit;
  }

  set OpenEdit(open: boolean){
    this.openEdit = open;
  }

  get ModalId(){
    return this.modalId;
  }

  set ModalId(id: number){
    this.modalId = id;
  }
  
  showMenu = signal(false);
  toggleMenu(): void {
    this.showMenu.update((showMenu) => !showMenu);
  }
}
