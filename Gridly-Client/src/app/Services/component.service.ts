import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {SetIconData, SetComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {firstValueFrom, Observable, take} from "rxjs";
import {IconModel} from "../Models/Icon.Model";
@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  components$!: Observable<ComponentModel[]>;
  components!: ComponentModel[];
  iconData!: IconModel;

  constructor(public endpointService: ComponentEndpointService) {
    this.components$ = this.endpointService.GetComponents();
    this.components$.pipe(take(1)).subscribe(data => this.components = data);
    this.editMode = false;
    this.dragMode = false;
    this.resizeMode = false;
  }

  SwitchEditMode(): void {
    this.editMode = !this.editMode;
    this.resizeMode = false;
    this.dragMode = false;
    this.components.forEach((component) => {
      component.editMode = this.editMode;
      component.dragMode = this.dragMode;
      component.resizeMode = this.resizeMode;
    });
  }

  SwitchResizeMode(): void {
    this.resizeMode = !this.resizeMode;
    this.dragMode = false;
    this.editMode = false;
    this.components.forEach((component) => {
      component.resizeMode = this.resizeMode;
      component.editMode = this.editMode;
      component.dragMode = this.dragMode;
    });
  }

  SwitchDragMode(): void {
    this.dragMode = !this.dragMode;
    this.editMode = false;
    this.resizeMode = false;
    this.components.forEach((component) => {
      component.dragMode = this.dragMode;
      component.editMode = this.editMode;
      component.resizeMode = this.resizeMode;
    });
  }

  AddComponent(component: ComponentModel) : boolean {
      this.endpointService.GetComponents().pipe(take(1))
        .subscribe((components) => {
          this.components = components.sort((a, b) => a.id - b.id);
        });
      debugger;
      component = SetComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});

      let index = this.endpointService.GetIndex(component.id);
      if(index === -1 && component.name !== "" && component.url !== "")
      {
        if(component.iconData != null || component.iconData !== undefined){
          component.iconData = SetIconData(component.iconData)
          if(component.iconData?.base64Data !== "" && component.iconData?.name !== "" && component.iconData?.fileType !== ""){
            this.endpointService.AddComponent(SetComponentData(undefined,{id: component.id, name: component.name, url: component.url, iconData: component.iconData, imageHidden: component.imageHidden, titleHidden: component.titleHidden}));
            return true;
          }
          if(component.imageUrl !== ""){
            this.endpointService.AddComponent(SetComponentData(undefined,{id: component.id, name: component.name, url: component.url, imageUrl: component.imageUrl, imageHidden: component.imageHidden, titleHidden: component.titleHidden}));

          }
          return true;
        }

      }
      else{
        this.AddComponent(component);
      }

    return false;
  }

  async GetComponents(): Promise<ComponentModel[]> {
    return await firstValueFrom(this.endpointService.GetComponents());
  }

  EditComponent(component: ComponentModel) {
    if(component.id !== 0)
      this.endpointService.EditComponent(component,component.iconData);
  }

  Remove(id: number): void {
    this.endpointService.RemoveComponent(id);
  }
}
