import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { verifyCode } from '../../models/verifyCode';
import { resetPassword } from '../../models/reset-password';

@Injectable({
  providedIn: 'root'
})
export class PasswordService {

  private UrlSendCodeToUser = "https://localhost:7233/api/Password/SendOtpPassword";
  private UrlSendCodeToServer = "https://localhost:7233/api/Password/check-verify-code-password";
  private UrlResetPassword = "https://localhost:7233/api/Password/Reset-password";

  constructor(private http: HttpClient) { }

  setResetToken(name: string ,token: string) {
    sessionStorage.setItem(name, token);
  }

  getResetToken(name: string): string | null {
    return sessionStorage.getItem(name);
  }

  SendCodeToUser(NameOrEmail: string): Observable<any> {
    const url = `${this.UrlSendCodeToUser}/${NameOrEmail}`
    return this.http.post<any>(url,NameOrEmail);
  }

  SendCodeToServer(verify: verifyCode): Observable<string> {
    return this.http.post<string>(this.UrlSendCodeToServer, verify);
  }

  ResetPassword(resetPassword: resetPassword): Observable<any> {
    return this.http.post<any>(this.UrlResetPassword, resetPassword);
  }
}
