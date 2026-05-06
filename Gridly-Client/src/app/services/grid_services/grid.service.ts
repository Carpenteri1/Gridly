import { Injectable, signal } from "@angular/core";

@Injectable({providedIn: 'root'})
export class GridService {
  private readonly inEditMode = signal(false);
  readonly editMode = this.inEditMode.asReadonly();

  toggle = () => this.inEditMode.update((value) => !value);
  setEditMode(enabled: boolean): void {
    this.inEditMode.set(enabled);
  }
}
