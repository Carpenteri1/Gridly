import { inject, Injectable, Signal } from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import { firstValueFrom, Observable, ReplaySubject, shareReplay, startWith, Subject, switchMap } from 'rxjs';
import { ComponentModel } from '../Models/Component.Model';
import { EditComponentModel } from '../Models/editComponent.Model';
import { ComponentEndpointService } from './endpoints/component.endpoint.service';

@Injectable({ providedIn: 'root' })
export class ComponentService {
  #api = inject(ComponentEndpointService);

  private readonly refreshTrigger = new Subject<void>();

  readonly componentId = new ReplaySubject<number>(1);
  readonly components$: Observable<ComponentModel[]> = this.refreshTrigger.pipe(
    startWith(void 0),
    switchMap(() => this.#api.get()),
    shareReplay(1)
  );

  readonly currentComponents: Signal<ComponentModel[] | undefined>;

  constructor() {
    this.currentComponents = toSignal(this.components$);
    this.refresh();
  }

  private edit$ = (component: EditComponentModel) => this.#api.edit(component);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  private add$ = (component: ComponentModel) => this.#api.add(component);

  edit = (component: ComponentModel) => firstValueFrom(this.edit$({editComponent: component, selectedDropDownIconValue: 2} as EditComponentModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (component: ComponentModel) => firstValueFrom(this.add$(component)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());

  refresh(): void {
    this.refreshTrigger.next();
  }
}
