import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize, take } from 'rxjs';

import { AuthService } from '../../../../core/services/auth.service';

const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value as string | undefined;
  const confirmPassword = control.get('confirmPassword')?.value as string | undefined;

  if (!password || !confirmPassword) {
    return null;
  }

  return password === confirmPassword ? null : { passwordMismatch: true };
};

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.page.html',
})
export class RegisterPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly registerForm = this.formBuilder.nonNullable.group(
    {
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      terms: [false, [Validators.requiredTrue]],
    },
    { validators: [passwordMatchValidator] },
  );

  isPasswordVisible = false;
  isLoading = false;
  errorMessage = '';

  get fullName() {
    return this.registerForm.controls.fullName;
  }

  get email() {
    return this.registerForm.controls.email;
  }

  get password() {
    return this.registerForm.controls.password;
  }

  get confirmPassword() {
    return this.registerForm.controls.confirmPassword;
  }

  get terms() {
    return this.registerForm.controls.terms;
  }

  togglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  submit(): void {
    this.clearEmailTakenError();

    if (this.registerForm.invalid || this.isLoading) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { fullName, email, password } = this.registerForm.getRawValue();

    this.authService
      .register(fullName, email, password)
      .pipe(
        take(1),
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (session) => {
          this.authService.storeSession(session, true);
          void this.router.navigateByUrl('/account');
        },
        error: (error: Error) => {
          if (error.message === 'Email already in use') {
            const currentErrors = this.email.errors ?? {};
            this.email.setErrors({ ...currentErrors, emailTaken: true });
            this.email.markAsTouched();
            return;
          }

          this.errorMessage = error.message || 'Registration failed';
        },
      });
  }

  private clearEmailTakenError(): void {
    const currentErrors = this.email.errors;
    if (!currentErrors?.['emailTaken']) {
      return;
    }

    const { emailTaken, ...remainingErrors } = currentErrors;
    this.email.setErrors(Object.keys(remainingErrors).length ? remainingErrors : null);
  }
}
