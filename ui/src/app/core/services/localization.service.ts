// src/app/core/services/localization.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap, map } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../config/api.config';

export type LangCode = 'en' | 'ar';

@Injectable({ providedIn: 'root' })
export class LocalizationService {

  private currentLang: LangCode = 'en';
  private messages: Record<string, string> = {};
  private loadedLang: LangCode | null = null;

  constructor(private http: HttpClient) {}

  load(lang: LangCode): Observable<void> {
    if (this.loadedLang === lang && Object.keys(this.messages).length > 0) {
      this.currentLang = lang;
      return of(void 0);
    }

    return this.http
      .get<Record<string, string>>(
        API_BASE_URL + API_ENDPOINTS.localization.byLang(lang)
      )
      .pipe(
        tap(dict => {
          this.messages = dict || {};
          this.currentLang = lang;
          this.loadedLang = lang;
        }),
        map(() => void 0)
      );
  }

  t(key: string): string {
    return this.messages[key] ?? key;
  }

  getLang(): LangCode {
    return this.currentLang;
  }
}
