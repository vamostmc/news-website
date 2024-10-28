import { CanActivateFn } from '@angular/router';
import { AuthenService } from '../Client/service-client/authen-service/authen.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';


export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenService); // Inject AuthenService
  const router = inject(Router); // Inject Router

  if (authService.isLoggedIn()) {
    return true; // Người dùng đã đăng nhập, cho phép truy cập
  } else {
    // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
    router.navigate(['/login']);
    return false; // Không cho phép truy cập
  }
  
};
