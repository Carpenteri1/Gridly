import {ApplicationConfig, inject, provideAppInitializer, provideEnvironmentInitializer, provideZoneChangeDetection} from '@angular/core';
import {provideHttpClient} from "@angular/common/http";
import {MatIconRegistry} from "@angular/material/icon";
import {LocaleStringsService} from "./services/locale_services/locale-strings.service";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(),
    provideEnvironmentInitializer(() => {
      inject(MatIconRegistry).setDefaultFontSetClass('material-symbols-outlined');
    }),
    provideAppInitializer(() => inject(LocaleStringsService).load()),
  ]
};
