import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize, take } from 'rxjs';

import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.page.html',
})
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loginForm = this.formBuilder.nonNullable.group({
    emailOrUsername: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    rememberMe: false,
  });

  isPasswordVisible = false;
  isLoading = false;
  errorMessage = '';

  get emailOrUsername() {
    return this.loginForm.controls.emailOrUsername;
  }

  get password() {
    return this.loginForm.controls.password;
  }

  togglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  submit(): void {
    if (this.loginForm.invalid || this.isLoading) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { emailOrUsername, password, rememberMe } = this.loginForm.getRawValue();

    this.authService
      .login(emailOrUsername, password)
      .pipe(
        take(1),
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (session) => {
          this.authService.storeSession(session, rememberMe);
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/account';
          void this.router.navigateByUrl(returnUrl);
        },
        error: (error: Error) => {
          this.errorMessage = error.message || 'Invalid credentials';
        },
      });
  }
}
