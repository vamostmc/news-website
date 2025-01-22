import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // Nếu trường rỗng, không kiểm tra (dựa vào validator required nếu có).
    }
    
    // Kiểm tra điều kiện: chữ hoa, chữ thường, số, và ký tự đặc biệt
    const hasUpperCase = /[A-Z]/.test(control.value);
    const hasLowerCase = /[a-z]/.test(control.value);
    const hasNumber = /\d/.test(control.value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(control.value);

    const valid = hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar;

    return valid ? null : { invalidPassword: 'Mật khẩu phải chứa chữ hoa, chữ thường, số và ký tự đặc biệt' };
  };
}
