import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class CookieStorageService {

  constructor(private cookieService: CookieService) { }

   // Lưu thông tin tài khoản vào cookie
   saveAccountCookie(username: string, password: string): void {
    const encryptedPassword = btoa(password);
    this.cookieService.set('username', username, 180); // Lưu trong 365 ngày
    this.cookieService.set('password', encryptedPassword, 180);
  }

  // Lấy tài khoản từ cookie
  getAccountCookie(): { username: string, password: string } | null {
    const username = this.cookieService.get('username');
    const password = this.cookieService.get('password');
    if (username && password) {
      return { username, password: atob(password) };
    }
    return null;
  }

  // Xóa tài khoản khỏi cookie
  clearAccountCookie(): void {
    this.cookieService.delete('username');
    this.cookieService.delete('password');
  }
}
