import { Injectable, inject } from '@angular/core';
import {urlConstants} from "../../constants/url.constants";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {RowColumnModel} from "../../models/rowColumn.Model";

@Injectable({
  providedIn: 'root'
})

export class RowColumnEndpointService{
  private http = inject(HttpClient);

  get(): Observable<RowColumnModel[] | null> {
    return this.http.get<RowColumnModel[] | null>(urlConstants.rowColumn.get).pipe(take(1));
  }
  batchSave(rows: RowColumnModel[]): Observable<RowColumnModel[]> {
    return this.http.post<RowColumnModel[]>(urlConstants.rowColumn.batchSave, rows).pipe(take(1));
  }
}
