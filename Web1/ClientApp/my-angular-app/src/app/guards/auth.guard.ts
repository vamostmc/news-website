import { CanActivateFn } from '@angular/router';
import { AuthenService } from '../Client/service-client/authen-service/authen.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmMailService } from '../Client/service-client/confirm-mail-service/confirm-mail.service';
import { of, switchMap } from 'rxjs';


// Chặn xem đã đăng nhập và xác nhận email chưa
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenService); // Inject AuthenService
  const confirmEmail = inject(ConfirmMailService); // Inject ConfirmMailService
  const router = inject(Router); // Inject Router

  // Xử lý bất đồng bộ với Observable
  return authService.isLoggedIn().pipe(
    switchMap((isLoggedIn) => {
      if (isLoggedIn) {
        console.log('User đã đăng nhập, kiểm tra xác nhận email...');

        // Kiểm tra xác nhận email
        if (!confirmEmail.isEmailConfirmed()) {
          console.log('Email chưa xác nhận, chuyển hướng confirmEmail');
          router.navigate(['/confirmEmail']);
          return of(false); // Không cho phép truy cập
        }

        // Đã đăng nhập và email đã xác nhận
        console.log('Email đã xác nhận, cho phép truy cập admin');
        return of(true); // Cho phép truy cập
      } else {
        console.log('User chưa đăng nhập, chuyển hướng login');
        router.navigate(['/login']);
        return of(false); // Không cho phép truy cập
      }
    })
  );
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


