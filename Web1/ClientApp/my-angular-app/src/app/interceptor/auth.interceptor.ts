import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthenService } from '../Client/service-client/authen-service/authen.service';
import { PlatformService } from '../Client/service-client/platform-service/platform.service';


@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthenService, 
              private platformService: PlatformService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let clonedRequest = req;

    if (this.platformService.isBrowser()) {
      const token = localStorage.getItem('accessToken');
      if (token) {
        clonedRequest = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
      }
    }


    // const token = localStorage.getItem('accessToken');
    // const clonedRequest = token 
    //   ? req.clone({ setHeaders: { Authorization: Bearer ${token} } })
    //   : req;

    return next.handle(clonedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        // Kiểm tra nếu là lỗi 401 (Unauthorized)
        if (error.status === 401) {
          return this.authService.GetRefreshToken().pipe(
            switchMap((newTokens: string) => {
              // Sau khi refresh token thành công, cập nhật lại access token
              localStorage.setItem('accessToken', newTokens);
              

              // Lấy lại request gốc và gửi lại
              const newRequest = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newTokens}`,
                },
              });
              return next.handle(newRequest);
            })
          );
        }
        return throwError(error);  // Trả về lỗi nếu không phải lỗi 401
      })
    );
  }
}
