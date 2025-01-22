import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmMailService {
  private SendCodeUrl = "https://localhost:7233/api/ConfirmEmail/send-verify-mail";
  private checkVerifyCodeURL = "https://localhost:7233/api/ConfirmEmail/check-verify-code";

  SendCodeToUser(IdUser: string): Observable<string> {
    const postURL = `${this.SendCodeUrl}/${IdUser}`;
    return this.http.post<string>(postURL,IdUser);
  }

  SendCodetoServer(codeFromMail: any): Observable<string> {
    return this.http.post<string>(this.checkVerifyCodeURL,codeFromMail);
  } 

  isEmailConfirmed(): boolean {
    // Giả sử bạn có một flag hoặc thông tin trong localStorage hoặc từ API
    const emailConfirmed = localStorage.getItem('confirmEmail'); // Hoặc sử dụng API nếu cần
    return emailConfirmed === 'true'; // Trả về true nếu đã xác nhận, false nếu chưa
  }

  

  constructor(private http: HttpClient) { }
}
