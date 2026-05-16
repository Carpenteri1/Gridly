import { Injectable, signal } from "@angular/core";

@Injectable({providedIn: 'root'})
export class GridService {
  private readonly _editMode = signal(false);
  readonly inEditMode = this._editMode.asReadonly();

  setEditMode = (value: boolean) => this._editMode.set(value);
  toggleEdit = () => this._editMode.update((value) => !value);
}
