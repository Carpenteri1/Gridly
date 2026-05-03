import { CardTypes } from "../Types/card.types.enum";
export class CardOptionModel {
  type!: CardTypes;
  label!: string;
  description!: string;
  icon!: string;
}
