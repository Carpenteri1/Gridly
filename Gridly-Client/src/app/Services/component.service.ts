import { inject, Injectable, Signal } from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import { firstValueFrom, Observable, ReplaySubject, shareReplay, startWith, Subject, switchMap } from 'rxjs';
import { CardModel } from '../Models/card.Model';
import { EditCardModel } from '../Models/editCard.Model';
import { ComponentEndpointService } from './endpoints/component.endpoint.service';

@Injectable({ providedIn: 'root' })
export class ComponentService {
  #api = inject(ComponentEndpointService);

  private readonly refreshTrigger = new Subject<void>();

  readonly componentId = new ReplaySubject<number>(1);
  readonly components$: Observable<CardModel[]>;

  readonly currentComponents: Signal<CardModel[] | undefined>;

  constructor() {
    this.components$ = this.get$;
    this.currentComponents = toSignal(this.components$);
    this.refresh();
  }

  private batchEdit$ = (components: EditCardModel[]) => this.#api.batchEdit(components);
  private edit$ = (component: EditCardModel) => this.#api.edit(component);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  private add$ = (component: CardModel) => this.#api.add(component);
  private get$ = this.refreshTrigger.pipe(startWith(void 0),switchMap(() => this.#api.get()),shareReplay(1));

  edit = (component: CardModel) => firstValueFrom(this.edit$({editComponent: component, selectedDropDownIconValue: 2} as EditCardModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (component: CardModel) => firstValueFrom(this.add$(component)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());

  refresh(): void {
    this.refreshTrigger.next();
  }
}
