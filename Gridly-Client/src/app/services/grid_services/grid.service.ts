import {inject, Injectable, Signal, signal} from "@angular/core";
import {BehaviorSubject, firstValueFrom, Observable, take} from "rxjs";
import {RowColumnModel} from "../../models/rowColumn.Model";
import {RowColumnEndpointService} from "../endpoint_services/rowColumn.endpoint.service";
import {toSignal} from "@angular/core/rxjs-interop";

@Injectable({providedIn: 'root'})
export class GridService {
  private readonly RowColumnSubject = new BehaviorSubject<RowColumnModel[]>([]);
  private readonly _editMode = signal(false);

  readonly rows$: Observable<RowColumnModel[]>;
  readonly inEditMode = this._editMode.asReadonly();
  readonly currentRowColumns: Signal<RowColumnModel[]>;

  #api = inject(RowColumnEndpointService);

  constructor() {
    this.rows$ = this.RowColumnSubject.asObservable();
    this.currentRowColumns = toSignal(this.rows$, { initialValue: [] as RowColumnModel[] });
    this.refresh();
  }

  private add$ = (rowColumn: RowColumnModel) => this.#api.add(rowColumn);

  add = (rowColumn: RowColumnModel) =>  firstValueFrom(this.add$(rowColumn)).then(() => this.refresh());

  setEditMode = (value: boolean) => this._editMode.set(value);
  toggleEdit = () => this._editMode.update((value) => !value);

  refresh(): void {
    this.#api.get().pipe(take(1))
      .subscribe((rowColumns) =>
        this.RowColumnSubject.next(rowColumns));
  }

  setRowsForView(rows: RowColumnModel[]): void {
    this.RowColumnSubject.next(rows);
  }

}
