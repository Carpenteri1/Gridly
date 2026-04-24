import {inject, Injectable, Signal} from "@angular/core";
import {toSignal} from '@angular/core/rxjs-interop';
import {ComponentModel} from "../Models/Component.Model";
import {EditComponentModel} from "../Models/editComponent.Model";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {firstValueFrom, Observable, ReplaySubject, switchMap, take} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService{
  #api = inject(ComponentEndpointService);
  
  componentId$ = new ReplaySubject<number>(1);
  component$: Observable<ComponentModel>;
  components$: Observable<ComponentModel[]>;

  readonly currentComponent: Signal<ComponentModel | undefined>;
  readonly currentComponents: Signal<ComponentModel[] | undefined>;

  constructor() {
    this.component$ = this.getById$();
    this.components$ = this.get$();

    this.currentComponent = toSignal(this.component$);
    this.currentComponents = toSignal(this.components$);
  }
  
  private edit$ = (component: EditComponentModel) => this.#api.edit(component).pipe(take(1));
  private getById$ = () => this.componentId$.pipe(switchMap(id => this.#api.getById(id)), take(1));
  private delete$ = (id: number) => this.#api.delete(id).pipe(take(1));
  private get$ = () => this.#api.get().pipe(take(1));
  private add$ = (component: ComponentModel) => this.#api.add(component).pipe(take(1));
  private batchEdit$ = (components: EditComponentModel[]) => this.#api.batchEdit(components).pipe(take(1));

  edit = (component: ComponentModel) => firstValueFrom(this.edit$({editComponent: component, selectedDropDownIconValue: 2} as EditComponentModel));
  add = (component: ComponentModel) => firstValueFrom(this.add$(component));
  delete = (id: number) => firstValueFrom(this.delete$(id));

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

  readonly #componentHasBaseData = true;

  get ComponentHasBaseData(){
    return this.#componentHasBaseData;
    /*
    return this.component.name !== undefined && "" &&
      this.component.url !== undefined && "" &&
      this.component.iconData !== undefined ||
      this.component.iconUrl !== undefined && "";*/
  }

  /*get IconIsUrlHidden(){
    return this.iconUrlHidden;
  }

  get IconIsFileHidden(){
    return this.iconHidden;
  }

  get ResizeModeActive(): boolean {
    return this.editMode;
  }*/

  CheckComponentData(item:ComponentModel): boolean {
    return item !== undefined &&
      item.name !== "" &&
      item.url !== "" &&
      RegexStringsUtil.urlPattern.test(item.url) &&
      RegexStringsUtil.namePattern.test(item.name);
  }
}
