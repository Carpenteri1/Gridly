import { inject, Injectable, Signal } from "@angular/core";
import { toSignal } from '@angular/core/rxjs-interop';
import { ComponentModel } from "../Models/Component.Model";
import { EditComponentModel } from "../Models/editComponent.Model";
import { ComponentEndpointService } from "./endpoints/component.endpoint.service";
import { BehaviorSubject, firstValueFrom, map, Observable, take } from "rxjs";
import { RegexStringsUtil } from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ComponentService {
  #api = inject(ComponentEndpointService);

  readonly #componentsState = new BehaviorSubject<ComponentModel[]>([]);
  readonly components$: Observable<ComponentModel[]> = this.#componentsState.asObservable();
  readonly component$: Observable<ComponentModel | undefined> = this.components$.pipe(
    map((components) => components[0]),
  );
  readonly currentComponents: Signal<ComponentModel[]>;

  constructor() {
    this.currentComponents = toSignal(this.components$, { initialValue: [] });
    void this.refresh();
  }

  async refresh(): Promise<ComponentModel[]> {
    const components = await firstValueFrom(this.#api.get().pipe(take(1)));
    this.#componentsState.next(components);
    return components;
  }

  componentById$(id: number): Observable<ComponentModel | undefined> {
    return this.components$.pipe(
      map((components) => components.find((component) => component.id === id)),
    );
  }

  getById(id: number): ComponentModel | undefined {
    return this.#componentsState.value.find((component) => component.id === id);
  }

  async edit(component: ComponentModel): Promise<ComponentModel> {
    const updatedComponent = await firstValueFrom(
      this.#api
        .edit({ editComponent: component, selectedDropDownIconValue: 2 } as EditComponentModel)
        .pipe(take(1)),
    );

    this.updateInState(updatedComponent);
    return updatedComponent;
  }

  async add(component: ComponentModel): Promise<ComponentModel> {
    const createdComponent = await firstValueFrom(this.#api.add(component).pipe(take(1)));
    this.#componentsState.next([...this.#componentsState.value, createdComponent]);
    return createdComponent;
  }

  async delete(id: number): Promise<ComponentModel> {
    const deletedComponent = await firstValueFrom(this.#api.delete(id).pipe(take(1)));
    this.#componentsState.next(
      this.#componentsState.value.filter((component) => component.id !== id),
    );
    return deletedComponent;
  }

  updateInState(component: ComponentModel): void {
    const components = this.#componentsState.value;
    const componentIndex = components.findIndex((item) => item.id === component.id);

    if (componentIndex === -1) {
      this.#componentsState.next([...components, component]);
      return;
    }

    const nextComponents = [...components];
    nextComponents[componentIndex] = component;
    this.#componentsState.next(nextComponents);
  }

  IconDataSet(item: ComponentModel): boolean {
    return item.iconData != undefined &&
      item.iconData.name !== "" &&
      item.iconData.type !== undefined &&
      item.iconData.base64Data !== "" &&
      !item.componentSettings?.imageHidden;
  }

  IconUrlSet(item: ComponentModel): boolean {
    return item.iconUrl !== undefined &&
      item.iconUrl !== "" &&
      RegexStringsUtil.iconUrlPattern.test(item.iconUrl) &&
      !item.componentSettings?.imageHidden;
  }

  MaterialIconSet(item: ComponentModel): boolean {
    return item.iconData?.materialIcon !== undefined &&
      item.iconData?.materialIcon !== "" &&
      !item.componentSettings?.imageHidden;
  }

  CheckData(item: ComponentModel): boolean {
    return item !== undefined &&
      item.name !== "" &&
      item.url !== "" &&
      RegexStringsUtil.urlPattern.test(item.url) &&
      RegexStringsUtil.namePattern.test(item.name);
  }
}
