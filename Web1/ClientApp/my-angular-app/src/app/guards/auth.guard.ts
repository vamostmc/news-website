import { CanActivateFn } from '@angular/router';
import { AuthenService } from '../Client/service-client/authen-service/authen.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmMailService } from '../Client/service-client/confirm-mail-service/confirm-mail.service';


// Chặn xem đã đăng nhập và xác nhận email chưa
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenService); // Inject AuthenService
  const confirmEmail = inject(ConfirmMailService);
  const router = inject(Router); // Inject Router

  if (authService.isLoggedIn()) {
    
    if (!confirmEmail.isEmailConfirmed()) {
      // Nếu email chưa xác nhận, điều hướng tới trang xác nhận email
      router.navigate(['/confirmEmail']);
      return false;
    }
    return true; 
  } else {
    router.navigate(['/login']);
    return false; 
  }
};

//Chặn khi người khác truy cập thay đổi password 
export const authResetToken: CanActivateFn = (route, state) => {
  const router = inject(Router); // Inject Router

  const resetToken = sessionStorage.getItem('ResetToken');
  console.log(`Giá trị của token server là: ${resetToken}`);
  const queryToken = route.paramMap.get('resetToken');
  console.log(`Giá trị của token máy này là: ${queryToken}`);
  if (resetToken && queryToken && resetToken === queryToken) {
    
    return true; // Token hợp lệ
  } else {
    // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
    router.navigate(['/login']);
    return false; // Không cho phép truy cập
  }
};


