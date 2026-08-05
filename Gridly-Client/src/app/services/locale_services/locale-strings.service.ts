import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { EnLocale } from '../../interfaces/en-locale.Interface';

@Injectable({
  providedIn: 'root'
})

export class LocaleStringsService {
  private http = inject(HttpClient);

  private data!: EnLocale;

  get locale(): EnLocale {
    return this.data;
  }

  async load(): Promise<void> {
    this.data = await firstValueFrom(this.http.get<EnLocale>('/assets/i18n/en.json'));
  }
}
