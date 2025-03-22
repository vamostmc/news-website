import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class ConfirmMailService {

  SendCodeToUser(IdUser: string): Observable<string> {
    const postURL = API_ENDPOINTS.emailVerification.sendVerifyMail(IdUser);
    return this.http.post<string>(postURL,IdUser);
  }

  SendCodetoServer(codeFromMail: any): Observable<string> {
    return this.http.post<string>(API_ENDPOINTS.emailVerification.checkVerifyCode,codeFromMail);
  } 

  //Kiểm tra email đã xác nhận chưa
  isEmailConfirmed(): boolean {
    const emailConfirmed = localStorage.getItem('confirmEmail'); 
    return emailConfirmed === 'true'; 
  }

  

  constructor(private http: HttpClient) { }
}
