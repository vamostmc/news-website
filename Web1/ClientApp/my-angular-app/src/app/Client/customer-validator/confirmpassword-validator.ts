import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function confirmPasswordValidator(passwordField: string, confirmPasswordField: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const password = control.get(passwordField)?.value;
    const confirmPassword = control.get(confirmPasswordField)?.value;

    return password && confirmPassword && password === confirmPassword
      ? null
      : { passwordMismatch: 'Mật khẩu xác nhận không khớp' };
  };
}
