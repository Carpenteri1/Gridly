import { inject, Injectable, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SearchIconsResultDto } from '../DTOs/SearchIconsResultDto';
import {
  BehaviorSubject,
  Observable,
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  of,
  shareReplay,
  switchMap,
} from 'rxjs';
import { IconEndpointService } from './endpoints/icon.endpoint.service';

@Injectable({ providedIn: 'root' })
export class IconService {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  readonly currentIcons: Signal<SearchIconsResultDto | null | undefined>;
  
  #iconEndpoints = inject(IconEndpointService);
  
  private searchInput = new BehaviorSubject<string>('');

  constructor() {
    this.icons$ = this.getIcons$(this.searchInput);
    this.currentIcons = toSignal(this.icons$);
  }

  search(input: string): void {
    this.searchInput.next(input);
  }

  private getIcons$(input$: Observable<string>): Observable<SearchIconsResultDto | null> {
    return input$.pipe(
      map((input) => input.trim()),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((input) =>
        input.length > 0
          ? this.#iconEndpoints.search(input).pipe(catchError(() => of(null)))
          : of(null),
      ),
      shareReplay({ bufferSize: 1, refCount: true }),
    );
  }
}
