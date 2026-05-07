import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { firstValueFrom, Observable, ReplaySubject, shareReplay, startWith, Subject, switchMap } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { EditCardModel } from '../../models/editCard.Model';
import { CardEndpointService } from '../endpoint_services/card.endpoint.service';

@Injectable({ providedIn: 'root' })
export class CardService {
  #api = inject(CardEndpointService);

  private readonly refreshTrigger = new Subject<void>();

  readonly cardId = new ReplaySubject<number>(1);
  readonly cards$: Observable<CardModel[]>;

  readonly currentCards: Signal<CardModel[] | undefined>;

  constructor() {
    this.cards$ = this.get$;
    this.currentCards = toSignal(this.get$);
    this.refresh();
  }

  private batchEdit$ = (cards: CardModel[]) => this.#api.batchEdit(cards);
  private edit$ = (card: EditCardModel) => this.#api.edit(card);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  private add$ = (card: CardModel) => this.#api.add(card);
  private get$ = this.refreshTrigger.pipe(startWith(void 0),switchMap(() => this.#api.get()),shareReplay(1));

  batchEdit = (cards: CardModel[]) => firstValueFrom(this.batchEdit$(cards)).then(() => this.refresh());
  edit = (card: CardModel) => firstValueFrom(this.edit$({editCard: card, selectedDropDownIconValue: 2} as EditCardModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (card: CardModel) => firstValueFrom(this.add$(card)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());

  refresh(): void {
    this.refreshTrigger.next();
  }
}
