import { inject, Injectable } from "@angular/core";
import { catchError, firstValueFrom, of } from "rxjs";
import { ClockModel } from "../../models/clock.Model";
import { ClockEndpointService } from "../endpoint_services/clock.endpoint.service";

@Injectable({ providedIn: 'root' })
export class ClockService {
  #api = inject(ClockEndpointService);

  private getClock$ = (timeZone: string) => this.#api.get(timeZone).pipe(catchError(() => of(null)));
  private getTimeApiData$ = (timeZone: string) => this.#api.getTimeApiData(timeZone);

  async resolveClockData(timeZone: string): Promise<ClockModel> {
    const cached = await firstValueFrom(this.getClock$(timeZone));
    if (cached) return cached;
    return firstValueFrom(this.getTimeApiData$(timeZone));
  }
}
