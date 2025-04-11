import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { roles } from '../../models/role';
import { FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthConfig, OAuthService } from 'angular-oauth2-oidc';
import { googleRequest } from '../../models/googleRequest';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';
import { PlatformService } from '../platform-service/platform.service';



@Injectable({
  providedIn: 'root'
})

export class AuthenService {

  constructor(private http: HttpClient,
              private oauthService: OAuthService,
              private platformService: PlatformService) {  }

  private userNameSource = new BehaviorSubject<string | null>(null);
  currentUserName = this.userNameSource.asObservable();

  setUserName(userName: string) {
    this.userNameSource.next(userName);
    if (this.platformService.isBrowser()) {
      localStorage.setItem('userName', userName);
    }
  }

  getUserName(key: string): string {
    if (this.platformService.isBrowser()) {
      return localStorage.getItem('userName') || '';
    }
    return '';
  }

  isLoggedIn(): Observable<boolean> {
    console.log('Kiểm tra trạng thái login');
  
    let hasToken = false;
    let isManager = false;
  
    if (this.platformService.isBrowser()) {
      hasToken = !!localStorage.getItem('accessToken');
      isManager = this.GetRoleUser() === "Manager";
    }
  
    console.log('Has token:', hasToken);
    console.log('Is Manager:', isManager);
  
    if (hasToken && isManager) {
      console.log('Điều kiện đã thỏa mãn, gọi checkAdmin');
      return this.checkAdmin().pipe(
        map(isAdmin => {
          console.log('Phản hồi từ checkAdmin:', isAdmin);
          return isAdmin.success === true;
        })
      );
    } else {
      console.log('Không đủ điều kiện');
      return of(false);
    }
  }
  
  

  checkAdmin(): Observable<any> {
    console.log('Gọi API checkAdmin');
    let headers: HttpHeaders = this.getHeaderToken();
    
    return this.http.get<any>(API_ENDPOINTS.authen.checkAdmin, { headers, withCredentials: true }).pipe(
      tap(response => {
        console.log('Kết quả từ checkAdmin:', response); // In ra kết quả trả về từ API
      }),
      catchError(error => {
        console.error('Lỗi khi gọi checkAdmin API:', error); // In lỗi nếu có
        return of({ success: false });  // Trả về false nếu có lỗi
      })
    );
  }
  

  
  
  //Giải mã token
  GetRoleUser(): any {
    if (this.platformService.isBrowser()) {
      const roles = localStorage.getItem("userRoles");
      if (roles) {
        const rolesArray = JSON.parse(roles);
        if (rolesArray.includes('Manager')) return "Manager";
        if (rolesArray.includes('Customer')) return "Customer";
      }
    }
    return null;
  }

  
  //Form Đăng kí người dùng
  SetFormRegister(postForm: FormGroup): FormData {
    let formData = new FormData();
    // Gửi dữ liệu từ form group sang form data
    formData.append("userName", postForm.get("userName")?.value); // Tên trường trùng khớp
    formData.append("dateUser", postForm.get("dateUser")?.value); // Định dạng ngày
    formData.append("address", postForm.get("address")?.value); // Tên trường trùng khớp
    formData.append("fullName", postForm.get("fullName")?.value); // Tên trường trùng khớp
    formData.append("email", postForm.get("email")?.value);
    formData.append("password", postForm.get("password")?.value); // Tên trường trùng khớp
    formData.append("confirmPassword", postForm.get("confirmPassword")?.value); // Tên trường ConfirmPassword
    return formData;
  }

  

  PostRegister(FormRegister: FormGroup): Observable<{ succeeded: boolean; errors: string[] }> {
    return this.http.post<{ succeeded: boolean; errors: string[] }>(
      API_ENDPOINTS.authen.register, 
      this.SetFormRegister(FormRegister)
    );
  }
  
  // Kiểm tra người dùng đăng nhập theo form
  PostLogin(FormLogin: any): Observable<any> {
    return this.http.post<any>(API_ENDPOINTS.authen.login, FormLogin, { withCredentials: true });
  }
  
  // Login theo Google
  LoginGoogle(request: googleRequest): Observable<any> {
    return this.http.post<any>(API_ENDPOINTS.authen.loginGoogle, request, { withCredentials: true });
  }

  getHeaderToken(): HttpHeaders {
    let headers = new HttpHeaders();
  
    if (!this.platformService.isBrowser()) {
      return headers.append('No-Access', 'Server-Side');
    }
  
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      headers = headers.set('Authorization', `Bearer ${accessToken}`);
    } else {
      headers = headers.set('No-AccessToken', 'true');
    }
  
    return headers;
  }

  LogOut(): Observable<any> {
    let headers: HttpHeaders = this.getHeaderToken();
    return this.http.get<any>(API_ENDPOINTS.authen.logout, { headers, withCredentials: true });
  }


   // Gửi request với Authorization header
  checkHeaderWithToken(): Observable<any> {
    let headers: HttpHeaders = this.getHeaderToken();
    return this.http.get(API_ENDPOINTS.authen.checkAdmin , { headers , withCredentials: true } );
  }

  GetRefreshToken(): Observable<string> {
    return this.http.get<string>(API_ENDPOINTS.authen.refreshToken, { 
      headers: this.getHeaderToken(), 
      withCredentials: true, 
      responseType: 'text' as 'json' 
    });

  }
}
