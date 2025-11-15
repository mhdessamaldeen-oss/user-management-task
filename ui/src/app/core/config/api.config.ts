
export const API_BASE_URL = 'http://localhost:5177/api'; 

export const API_ENDPOINTS = {
  auth: {
    login: '/v1/auth/login',   
  },
  users: {
    list: '/v1/users',
    byId: (id: string | number) => `/v1/users/${id}`,
    create: '/v1/users',
    update: (id: string | number) => `/v1/users/${id}`,
    delete: (id: string | number) => `/v1/users/${id}`,
  }
};
