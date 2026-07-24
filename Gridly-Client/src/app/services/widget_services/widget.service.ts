import {inject, Injectable} from "@angular/core";
import {WidgetEndpointService} from "../endpoint_services/widget.endpoint.service";
import {firstValueFrom} from "rxjs";

@Injectable({providedIn: 'root'})
export class WidgetService {
  readonly #api = inject(WidgetEndpointService)
  private get$ = () => this.#api.get();
  get = () =>
    firstValueFrom(this.get$());
}

