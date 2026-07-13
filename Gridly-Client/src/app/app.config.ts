import {ApplicationConfig, inject, provideEnvironmentInitializer, provideZoneChangeDetection} from '@angular/core';
import {provideHttpClient} from "@angular/common/http";
import {MatIconRegistry} from "@angular/material/icon";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(),
    provideEnvironmentInitializer(() => {
      inject(MatIconRegistry).setDefaultFontSetClass('material-symbols-outlined');
    }),
  ]
};
