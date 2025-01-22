import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function emailValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // If the field is empty, don't validate (rely on required validator).
    }
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{3,}$/;
    const valid = emailPattern.test(control.value);

    return valid ? null : { invalidEmail: 'Email không đúng định dạng' };
  };
}
