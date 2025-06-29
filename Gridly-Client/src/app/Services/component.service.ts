import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {SetComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {Observable, take} from "rxjs";
import {IconModel} from "../Models/Icon.Model";
@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  components$!: Observable<ComponentModel[]>;
  components!: ComponentModel[];
  component!: ComponentModel;
  iconData!: IconModel;
  constructor(public componentEndpointService: ComponentEndpointService) {
    this.components$ = this.componentEndpointService.GetComponents();
    this.components$.pipe(take(1)).subscribe(data => this.components = data);
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

  AddComponent(component: ComponentModel) : boolean {
      component = SetComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});
      let index = this.componentEndpointService.GetIndex(component.id);;
      if(index === -1 && component.name !== "" && component.url !== "")
      {
        if(component.iconData != null || component.iconData !== undefined){
          if(component.iconData?.base64Data !== "" && component.iconData?.name !== "" && component.iconData?.type !== ""){
            this.componentEndpointService.AddComponent(SetComponentData(component)).pipe(take(1)).subscribe({
              next: (res) => console.log('Component saved!', res),
              error: (err) => console.error('Save failed:', err)
            });
            return true;
          }
          if(component.imageUrl !== ""){
            this.componentEndpointService.AddComponent(SetComponentData(component)).pipe(take(1)).subscribe({
              next: (res) => console.log('Component saved!', res),
              error: (err) => console.error('Save failed:', err)
            });
            return true;
          }
        }
      }
      else{
        this.AddComponent(component);
      }

    return false;
  }

  EditComponent(component: ComponentModel): boolean {
    if(component.id !== 0){
      this.componentEndpointService.EditComponent(component,component.iconData);
      return true;
    }
    return false;
  }

  EditComponents(editedComponents: ComponentModel[]) {
    for (let i = editedComponents.length - 1; i >= 0; i--) {
      for (let j = editedComponents.length - 1; j >= 0; j--) {
        if(this.components[i].componentSettings?.height != editedComponents[i].componentSettings?.height ||
        this.components[i].componentSettings?.width != editedComponents[i].componentSettings?.width){
          this.componentEndpointService.EditComponent(editedComponents[i],this.components[i].iconData);
        }
        //TODO is component index same as edited compoent index
      }
    }
  }

  DeleteComponent(id: number): boolean {
    let success = true;

    this.componentEndpointService.Delete(id).pipe(take(1)).subscribe({
      next: (res) => {
        if (res == null) {  // adjust depending on your API shape
          console.warn('Response is null or invalid:', res);
          success = false;
        } else {
          console.log('Component deleted!', res);
        }
      },
      error: (err) => {
        console.error('Delete failed:', err);
        success = false;
      }
    });
    return success;
  }
}
