import {inject, Injectable, Signal} from "@angular/core";
import {VersionModel} from "../models/version.Model";
import {VersionEndpointService} from "./endpoints/version.endpoint.service";
import {Observable} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";

@Injectable({providedIn: 'root'})
export class VersionService {
    #api = inject(VersionEndpointService);

    version$: Observable<VersionModel>;

    readonly currentVersion: Signal<VersionModel | undefined>;

    constructor() {
        this.version$ = this.getVersion$();
        this.currentVersion = toSignal(this.version$);
    }

    getVersion$ = () => this.#api.get();
}
