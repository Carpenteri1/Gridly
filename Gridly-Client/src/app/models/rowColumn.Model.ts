import {CardModel} from "./card.Model";

export class RowColumnModel {
  id!: number;
  cards: CardModel[] = [];
  rowPosition?: number;
  rowWidth?: number;
}
