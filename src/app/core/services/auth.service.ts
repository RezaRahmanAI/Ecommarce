import { Injectable } from '@angular/core';
import { defer, Observable, of, throwError } from 'rxjs';

export interface AuthUser {
  id: string;
  name: string;
  email: string;
}

export interface AuthSession {
  token: string;
  user: AuthUser;
}

interface DemoUser extends AuthUser {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly storageKey = 'auth_session';
  private readonly usersStorageKey = 'demo_users';

  private demoUsers: DemoUser[] = [];

  constructor() {
    this.demoUsers = this.loadUsers();
  }

  login(emailOrUsername: string, password: string): Observable<AuthSession> {
    return defer(() => {
      const matchedUser = this.demoUsers.find(
        (user) =>
          (user.email.toLowerCase() === emailOrUsername.toLowerCase() ||
            user.username.toLowerCase() === emailOrUsername.toLowerCase()) &&
          user.password === password,
      );

      if (!matchedUser) {
        return throwError(() => new Error('Invalid credentials'));
      }

      const session: AuthSession = {
        token: `mock-token-${matchedUser.id}`,
        user: {
          id: matchedUser.id,
          name: matchedUser.name,
          email: matchedUser.email,
        },
      };

      return of(session);
    });
  }

  register(fullName: string, email: string, password: string): Observable<AuthSession> {
    return defer(() => {
      const normalizedEmail = email.trim().toLowerCase();
      const existingUser = this.demoUsers.find(
        (user) => user.email.toLowerCase() === normalizedEmail,
      );

      if (existingUser) {
        return throwError(() => new Error('Email already in use'));
      }

      const baseUsername =
        normalizedEmail.split('@')[0] ||
        fullName
          .trim()
          .toLowerCase()
          .replace(/\\s+/g, '');
      let username = baseUsername || `user${this.demoUsers.length + 1}`;
      let suffix = 1;

      while (this.demoUsers.some((user) => user.username.toLowerCase() === username.toLowerCase())) {
        username = `${baseUsername}${suffix}`;
        suffix += 1;
      }

      const newUser: DemoUser = {
        id: `user-${this.demoUsers.length + 1}`,
        name: fullName.trim(),
        email: normalizedEmail,
        username,
        password,
      };

      this.demoUsers.push(newUser);
      this.storeUsers();

      const session: AuthSession = {
        token: `mock-token-${newUser.id}`,
        user: {
          id: newUser.id,
          name: newUser.name,
          email: newUser.email,
        },
      };

      return of(session);
    });
  }

  storeSession(session: AuthSession, rememberMe: boolean): void {
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem(this.storageKey, JSON.stringify(session));
  }

  updateSession(session: AuthSession): void {
    if (localStorage.getItem(this.storageKey)) {
      localStorage.setItem(this.storageKey, JSON.stringify(session));
      return;
    }
    if (sessionStorage.getItem(this.storageKey)) {
      sessionStorage.setItem(this.storageKey, JSON.stringify(session));
      return;
    }
    localStorage.setItem(this.storageKey, JSON.stringify(session));
  }

  clearSession(): void {
    localStorage.removeItem(this.storageKey);
    sessionStorage.removeItem(this.storageKey);
  }

  getSession(): AuthSession | null {
    const stored = localStorage.getItem(this.storageKey) ?? sessionStorage.getItem(this.storageKey);
    if (!stored) {
      return null;
    }

    try {
      return JSON.parse(stored) as AuthSession;
    } catch {
      return null;
    }
  }

  isAuthenticated(): boolean {
    return this.getSession() !== null;
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated() || localStorage.getItem('is_logged_in') === 'true';
  }

  getRole(): string {
    return localStorage.getItem('user_role') ?? 'user';
  }

  logout(): void {
    this.clearSession();
    localStorage.removeItem('is_logged_in');
    localStorage.removeItem('user_role');
  }

  private loadUsers(): DemoUser[] {
    const stored = localStorage.getItem(this.usersStorageKey);
    if (stored) {
      try {
        return JSON.parse(stored) as DemoUser[];
      } catch {
        // fall through to defaults
      }
    }

    const defaults: DemoUser[] = [
      {
        id: 'user-1',
        name: 'Amina Noor',
        email: 'amina@example.com',
        username: 'amina',
        password: 'password123',
      },
      {
        id: 'user-2',
        name: 'Layla Farah',
        email: 'layla@example.com',
        username: 'layla',
        password: 'style2024',
      },
    ];

    localStorage.setItem(this.usersStorageKey, JSON.stringify(defaults));
    return defaults;
  }

  private storeUsers(): void {
    localStorage.setItem(this.usersStorageKey, JSON.stringify(this.demoUsers));
  }
}
