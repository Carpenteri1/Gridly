import {inject, Injectable, Signal, signal} from "@angular/core";
import {toSignal} from '@angular/core/rxjs-interop';
import {ComponentModel} from "../Models/Component.Model";
import {ComponentEndpointService} from "./endpoints/component.endpoint.service";
import {Observable, ReplaySubject, switchMap, take} from "rxjs";
import {RegexStringsUtil} from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService{
  public editMode!: boolean;
  private iconHidden!: boolean;
  private iconUrlHidden!: boolean;
  private openEdit!: boolean;
  private modalId!:number;

  #api = inject(ComponentEndpointService);
  
  componentId$ = new ReplaySubject<number>(1);
  component$: Observable<ComponentModel>;
  components$: Observable<ComponentModel[]>;

  readonly currentComponent: Signal<ComponentModel | undefined>;
  readonly currentComponents: Signal<ComponentModel[] | undefined>;

  constructor() {
    this.editMode = false;

    this.component$ = this.getById$();
    this.components$ = this.get$();

    this.currentComponent = toSignal(this.component$);
    this.currentComponents = toSignal(this.components$);
  }

  getById$ = () => this.componentId$.pipe(switchMap(id => this.#api.getById(id)), take(1));
  delete$ = () => this.componentId$.pipe(switchMap(id => this.#api.delete(id)), take(1));
  edit$ = () => this.component$.pipe(switchMap(component => this.#api.edit(component)), take(1));
  get$ = () => this.#api.get().pipe(take(1));
  add$ = (component: ComponentModel) => this.#api.add(component).pipe(take(1)); 
  batchEdit$ = (components: ComponentModel[]) => this.#api.batchEdit(components).pipe(take(1));

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
    return true;
    /*
    return this.component.name !== undefined && "" &&
      this.component.url !== undefined && "" &&
      this.component.iconData !== undefined ||
      this.component.iconUrl !== undefined && "";*/
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
