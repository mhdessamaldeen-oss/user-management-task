// src/app/core/config/api.config.ts
export const API_BASE_URL = 'http://localhost:5177/api/v1';

export const API_ENDPOINTS = {
  auth: {
    login: '/auth/login',
  },

  users: {
    list: '/users',
    byId: (id: string | number) => `/users/${id}`,
    create: '/users',
    update: (id: string | number) => `/users/${id}`,
    delete: (id: string | number) => `/users/${id}`,
    dt: '/users/dt'
  },

  localization: {
    byLang: (lang: string) => `/localization/${lang}`
  }
};
