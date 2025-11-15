import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, map } from 'rxjs/operators';

export type LangCode = 'en' | 'ar';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private currentLang: LangCode = 'en';
  private messages: Record<string, string> = {};
  private loadedLang: LangCode | null = null;

  // adjust base URL if needed
  private baseUrl = 'http://localhost:5177/api/v1/localization';

  constructor(private http: HttpClient) {}

  load(lang: LangCode): Observable<void> {
    if (this.loadedLang === lang && Object.keys(this.messages).length > 0) {
      this.currentLang = lang;
      return of(void 0);
    }

    return this.http
      .get<Record<string, string>>(`${this.baseUrl}/${lang}`)
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
