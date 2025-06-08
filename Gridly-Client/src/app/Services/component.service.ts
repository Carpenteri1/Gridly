import {Injectable} from "@angular/core";
import {IComponentModel} from "../Models/IComponent.Model";
import {IIconModel} from "../Models/IIcon.Model";
import {CreateComponentData} from "../Utils/componentModal.factory";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {firstValueFrom, Observable} from "rxjs";
@Injectable({providedIn: 'root'})
export class ComponentService{
  editMode!: boolean;
  dragMode!: boolean;
  resizeMode!: boolean;
  component!: IComponentModel;
  components$!: Observable<IComponentModel[]>;
  components!: IComponentModel[];
  iconData!: IIconModel;

  constructor(public endpointService: ComponentEndpointService) {
    this.components$ = this.endpointService.GetComponents();
    this.components$.subscribe(data => this.components = data);
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

  AddComponent(newId?: number) : boolean {
    this.endpointService.GetComponents()
      .subscribe((components) => {
        this.components = components.sort((a, b) => a.id - b.id);
      });
    newId = Math.max(...this.components.map(x => x.id)) + 1;
    let index = this.endpointService.GetIndex(newId);
    if(index === -1 && this.component.name !== "" && this.component.url !== "")
    {
      if(this.iconData.base64Data !== "" && this.iconData.name !== "" && this.iconData.fileType !== ""){
        this.endpointService.AddComponent(CreateComponentData({id: newId, name: this.component.name, url: this.component.url, iconData: this.component.iconData, imageHidden: this.component.imageHidden, titleHidden: this.component.titleHidden}));
        return true;
      }
      if(this.component.imageUrl !== ""){
        this.endpointService.AddComponent(CreateComponentData({id: newId, name: this.component.name, url: this.component.url, imageUrl: this.component.imageUrl, imageHidden: this.component.imageHidden, titleHidden: this.component.titleHidden}));
        return true;
      }
      this.component = CreateComponentData();
    }
    else
      this.AddComponent(newId++);

    return false;
  }

  async GetComponents(): Promise<IComponentModel[]> {
    return await firstValueFrom(this.endpointService.GetComponents());
  }

  EditComponent() {
    this.endpointService.EditComponent(this.component,this.component.iconData);
  }
  Remove(id: number): void {
    this.endpointService.RemoveComponent(id);
  }
}
