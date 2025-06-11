import {Injectable} from "@angular/core";
import {ComponentModel} from "../Models/Component.Model";
import {SetComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {firstValueFrom, Observable, take} from "rxjs";
@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  components$!: Observable<ComponentModel[]>;
  components!: ComponentModel[];
  component!: ComponentModel;
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
      this.componentEndpointService.GetComponents().pipe(take(1))
        .subscribe((components) => {
          this.components = components.sort((a, b) => a.id - b.id);
        });
      component = SetComponentData(component,{id: Math.max(...this.components.map(x => x.id)) + 1});

      let index = this.componentEndpointService.GetIndex(component.id);
      if(index === -1 && component.name !== "" && component.url !== "")
      {
        if(component.iconData != null || component.iconData !== undefined){
          if(component.iconData?.base64Data !== "" && component.iconData?.name !== "" && component.iconData?.fileType !== ""){
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

  async GetComponents(): Promise<ComponentModel[]> {
    return await firstValueFrom(this.componentEndpointService.GetComponents());
  }

  EditComponent(component: ComponentModel) {
    if(component.id !== 0)
      this.componentEndpointService.EditComponent(component,component.iconData);
  }

  DeleteComponent(id: number): void {
    this.componentEndpointService.Delete(id).pipe(take(1)).subscribe({
      next: (res) => console.log('Component deleted!', res),
      error: (err) => console.error('Delete failed:', err)
    });
  }
}
