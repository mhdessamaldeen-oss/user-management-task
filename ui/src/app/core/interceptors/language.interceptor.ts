// src/app/core/interceptors/language.interceptor.ts
import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn
} from '@angular/common/http';
import { LocalizationService } from '../services/localization.service';

// Functional interceptor (Angular 16+ / 20 style)
export const languageInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {
  const loc = inject(LocalizationService);

  // Get current language from service (en / ar)
  const lang = loc.getLang() || 'en';

  const cloned = req.clone({
    setHeaders: {
      'Accept-Language': lang
    }
  });

  return next(cloned);
};
