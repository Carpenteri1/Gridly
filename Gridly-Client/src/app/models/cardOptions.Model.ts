import { CardTypes } from "../types/card.types.enum";
export class CardOptionModel {
  type!: CardTypes;
  label!: string;
  description!: string;
  icon!: string;
}
