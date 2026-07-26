import {inject, Injectable, Signal} from "@angular/core";
import {VersionModel} from "../../models/version.Model";
import {VersionEndpointService} from "../endpoint_services/version.endpoint.service";
import {catchError, Observable, of} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";

@Injectable({providedIn: 'root'})
export class VersionService {
    #api = inject(VersionEndpointService);

    version$: Observable<VersionModel | undefined>;

    readonly currentVersion: Signal<VersionModel | undefined>;

    constructor() {
        this.version$ = this.getVersion$();
        this.currentVersion = toSignal(this.version$);
    }

    getVersion$ = () => this.#api.get().pipe(catchError(() => of(undefined)));
}
