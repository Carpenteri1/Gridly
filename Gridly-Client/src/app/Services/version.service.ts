import {inject, Injectable, Signal} from "@angular/core";
import {VersionModel} from "../Models/Version.Model";
import {VersionEndpointService} from "./endpoints/version.endpoint.service";
import {lastValueFrom, Observable} from "rxjs";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {version} from '../../../package.json';
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
