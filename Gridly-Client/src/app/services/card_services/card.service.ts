import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject, firstValueFrom, Observable, ReplaySubject, take } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { EditCardModel } from '../../models/editCard.Model';
import { CardEndpointService } from '../endpoint_services/card.endpoint.service';

@Injectable({ providedIn: 'root' })
export class CardService {
  #api = inject(CardEndpointService);

  private readonly cardsSubject = new BehaviorSubject<CardModel[]>([]);

  readonly cardId = new ReplaySubject<number>(1);
  readonly cards$: Observable<CardModel[]>;

  readonly currentCards: Signal<CardModel[]>;

  constructor() {
    this.cards$ = this.cardsSubject.asObservable();
    this.currentCards = toSignal(this.cards$, { initialValue: [] as CardModel[] });
    this.refresh();
  }

  private batchEdit$ = (cards: CardModel[]) => this.#api.batchEdit(cards);
  private edit$ = (card: EditCardModel) => this.#api.edit(card);
  private getById$ = (id: number) => this.#api.getById(id);
  private delete$ = (id: number) => this.#api.delete(id);
  private add$ = (card: CardModel) => this.#api.add(card);

  batchEdit = (cards: CardModel[]) => firstValueFrom(this.batchEdit$(cards)).then(() => this.refresh());
  edit = (card: CardModel) => firstValueFrom(this.edit$({editCard: card, selectedDropDownIconValue: 2} as EditCardModel)).then(() => this.refresh());
  getById = (id: number) => firstValueFrom(this.getById$(id));
  add = (card: CardModel) =>  firstValueFrom(this.add$(card)).then(() => this.refresh());
  delete = (id: number) => firstValueFrom(this.delete$(id)).then(() => this.refresh());

  update = (card: CardModel): void => {
    const updatedCards = this.currentCards().map((currentCard) => {
      if (currentCard.id !== card.id) return currentCard;

      return {
        ...currentCard,
        ...card,
        settings: {
          width: card.settings?.width ?? currentCard.settings?.width ?? 250,
          height: card.settings?.height ?? currentCard.settings?.height ?? 250,
          imageHidden: card.settings?.imageHidden ?? currentCard.settings?.imageHidden ?? false,
          titleHidden: card.settings?.titleHidden ?? currentCard.settings?.titleHidden ?? false,
        },
      };
    });
    this.cardsSubject.next(updatedCards);
  };

  refresh(): void {
    this.#api.get().pipe(take(1))
      .subscribe((cards) =>
        this.cardsSubject.next(cards));
  }
}
