import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { API_CONFIG, ApiConfig } from '../config/api.config';
import { AuthSessionService } from '../services/auth-session.service';

const isApiRequest = (url: string, baseUrl: string): boolean => {
  if (url.startsWith('/')) {
    return url.startsWith(baseUrl) || url.startsWith('/api');
  }

  return url.startsWith(baseUrl);
};

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const sessionService = inject(AuthSessionService);
  const config = inject<ApiConfig>(API_CONFIG);
  const token = sessionService.getAccessToken();
  const baseUrl = config.baseUrl.replace(/\/$/, '');

  if (!token || !isApiRequest(request.url, baseUrl)) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    }),
  );
};
