import {CurrentConditionsModel} from "./currentConditions.Model";

export class WeatherModel {
  cardId!: number;
  location!: string;
  address!: string;
  timezone!: string;
  description!: string;
  currentConditions!: CurrentConditionsModel;
}
