import {inject, Injectable, Signal} from "@angular/core";
import {toSignal} from '@angular/core/rxjs-interop';
import {ComponentModel} from "../Models/Component.Model";
import {EditComponentModel} from "../Models/editComponent.Model";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {firstValueFrom, Observable, ReplaySubject, shareReplay, switchMap, Subject, startWith} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService{
  #api = inject(ComponentEndpointService);

  private refreshTrigger = new Subject<void>();
  
//  components$: Observable<ComponentModel[]>;

  componentId = new ReplaySubject<number>(1);

  readonly currentComponents: Signal<ComponentModel[] | undefined>;

  constructor() {
    //this.components$ = this.get$();
    this.currentComponents = toSignal(this.components$);
    this.refresh();
  }

  components$: Observable<ComponentModel[]> = this.refreshTrigger.pipe(
    startWith(null), // ← Emit null to trigger initial load
    switchMap(() => this.#api.get()),
    shareReplay(1)
  );
  
  private edit$ = (component: EditComponentModel) => this.#api.edit(component);
  private batchEdit$ = (components: EditComponentModel[]) => this.#api.batchEdit(components);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  //private get$ = () => this.refreshTrigger.asObservable().pipe(switchMap(() => this.#api.get()), startWith([]), shareReplay(1));
  private add$ = (component: ComponentModel) => this.#api.add(component);

  edit = (component: ComponentModel) => firstValueFrom(this.edit$({editComponent: component, selectedDropDownIconValue: 2} as EditComponentModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (component: ComponentModel) => firstValueFrom(this.add$(component)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());
  refresh(): void { this.refreshTrigger.next(); }
  
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
