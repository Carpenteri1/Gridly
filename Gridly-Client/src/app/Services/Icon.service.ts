import { inject, Injectable } from "@angular/core";
import { SearchIconsResultDto } from "../DTOs/SearchIconsResultDto";
import { BehaviorSubject, catchError, debounceTime, filter, Observable, of, switchMap } from 'rxjs';
import { IconEndpointService } from "./endpoints/icon.endpoint.service";

@Injectable({providedIn: 'root'})
export class IconService{
    icons$: Observable<SearchIconsResultDto | null>;
    searchInput$ = new BehaviorSubject<string>('');
    #iconEndpoints = inject(IconEndpointService);

    constructor() {
    this.icons$ = this.getIcons$(this.searchInput$);
  }

    getIcons$ (input$: Observable<string>): Observable<SearchIconsResultDto | null>{
      const result = input$.pipe(
        debounceTime(1000),
        filter(input => input.length > 0),
        switchMap(input => this.#iconEndpoints.search(input).pipe(catchError(() => of(null))))
      );
      return result;
    }
}
