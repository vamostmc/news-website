import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { verifyCode } from '../../models/verifyCode';
import { resetPassword } from '../../models/reset-password';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class PasswordService {

  constructor(private http: HttpClient) { }

  setResetToken(name: string ,token: string) {
    sessionStorage.setItem(name, token);
  }

  getResetToken(name: string): string | null {
    return sessionStorage.getItem(name);
  }

  SendCodeToUser(NameOrEmail: string): Observable<any> {
    const url = API_ENDPOINTS.passwordReset.sendOtpPassword(NameOrEmail);
    return this.http.post<any>(url,NameOrEmail);
  }

  SendCodeToServer(verify: verifyCode): Observable<string> {
    return this.http.post<string>(API_ENDPOINTS.passwordReset.checkVerifyCodePassword, verify);
  }

  ResetPassword(resetPassword: resetPassword): Observable<any> {
    return this.http.post<any>(API_ENDPOINTS.passwordReset.resetPassword, resetPassword);
  }
}
