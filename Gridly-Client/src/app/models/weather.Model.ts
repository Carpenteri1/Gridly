import {CurrentConditionsModel} from "./currentConditions.Model";

export class WeatherModel {
  location!: string;
  address!: string;
  timezone!: string;
  description!: string;
  currentConditions!: CurrentConditionsModel;
}
