import {Injectable, signal} from "@angular/core";

@Injectable({providedIn: 'root'})
export class GridService {
    private inEditMode = signal(false);
    toggle = () => this.inEditMode.update((value) => !value);
    getEditMode = () => this.inEditMode();
}
