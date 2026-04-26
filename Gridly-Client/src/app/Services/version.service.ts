import { inject, Injectable, Signal } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { shareReplay } from "rxjs";
import { VersionModel } from "../Models/Version.Model";
import { VersionEndpointService } from "./endpoints/version.endpoint.service";

@Injectable({providedIn: 'root'})
export class VersionService {
  #api = inject(VersionEndpointService);

  readonly version$ = this.#api.get().pipe(
    shareReplay({ bufferSize: 1, refCount: true }),
  );
  readonly currentVersion: Signal<VersionModel | undefined>;

  constructor() {
    this.currentVersion = toSignal(this.version$);
  }
}
