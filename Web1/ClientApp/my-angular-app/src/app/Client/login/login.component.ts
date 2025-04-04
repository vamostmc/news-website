import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { jwtDecode } from 'jwt-decode';
import { roles } from '../models/role';
import { log } from 'console';
import { FacebookLoginProvider, GoogleLoginProvider, GoogleSigninButtonModule, SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { CookieStorageService } from '../service-client/cookie-service/cookie-storage.service';
import { googleRequest } from '../models/googleRequest';
import { PlatformService } from '../service-client/platform-service/platform.service';

declare var google: any;
declare var FB: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
    constructor(private form: FormBuilder, 
                private http: HttpClient,
                private router: Router,
                private authService: AuthenService,
                private cookieService: CookieStorageService,
                private oAthService: SocialAuthService,
                private cdr: ChangeDetectorRef,
                private platformService: PlatformService) {}

    errorMessage: string | null = null;
    loading: boolean = true;
    passwordFieldType: string = 'password';  // Mặc định là ẩn mật khẩu
    passwordVisible: boolean = false;  // Mặc định là không hiển thị mật khẩu
    passwordEntered: boolean = false;
    isLoggedIn: boolean =  false;
    user!: SocialUser;
    isLogginGoogle: boolean = false;
    
    private URL = "https://localhost:7233/api/Account/login-google";

    loginForm = this.form.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      remember: [false]
    });

    //Chờ Google API tải xong
    loadGoogleSDK(): Promise<void> {
      return new Promise((resolve, reject) => {
        if (typeof google !== 'undefined' && google.accounts) {
          resolve();
        } else {
          const script = document.createElement('script');
          script.src = "https://accounts.google.com/gsi/client";
          script.async = true;
          script.defer = true;
          script.onload = () => resolve();
          script.onerror = () => reject("Lỗi khi tải Google API");
          document.body.appendChild(script);
        }
      });
    }
    

    // Hàm để khởi tạo Google Sign-In
    initializeGoogleLogin(): void {
      this.loadGoogleSDK().then(() => {
        if (typeof google !== 'undefined') {
          google.accounts.id.initialize({
            client_id: '938095493121-gf602qo4lrm6r0bb0pgqu5aafsn1prrq.apps.googleusercontent.com',
            callback: (response: any) => this.handleCredentialResponse(response),
            ux_mode: 'popup',
            context: 'signin',
            prompt_parent_id: "google-signin-btn",
            auto_select: false,
            itp_support: true,
            scope: 'profile email https://www.googleapis.com/auth/user.addresses.read https://www.googleapis.com/auth/user.birthday.read'
          });
    
          google.accounts.id.renderButton(
            document.getElementById("google-signin-btn"),
            {
              theme: "outline",
              size: "large"
            }
          );
        } else {
          console.error("Google API chưa sẵn sàng.");
        }
      }).catch((error) => console.error(error));
    }
    

  

  // Hàm xử lý khi đăng nhập thành công
  handleCredentialResponse(response: any): void {
    console.log("Encoded JWT ID token: " + response.credential);
    const tokenrequest : googleRequest = {
      token: response.credential,
    }

    this.errorMessage = null;
    if (this.authService && this.authService.LoginGoogle) {
      this.authService.LoginGoogle(tokenrequest).subscribe(
        (data) => {
          if(data.success == true) {
            console.log(data);
            this.SaveLocalStorage(data);
            window.location.reload();
          }
          else {
            console.log(data);
            this.errorMessage = "Email tồn tại đã dùng đăng kí";
            this.cdr.detectChanges();
          }
          
        },
        (error) => {
          console.error('Error during login', error);
        }
      );
    } else {
      console.error('authService or LoginGoogle is undefined');
    }
  }

    
    signInWithGoogle(): void {
      this.oAthService.signIn(GoogleLoginProvider.PROVIDER_ID).then(user => {
        this.user = user;
        this.isLogginGoogle = true;
        console.log('Đăng nhập thành công:', user);
      }).catch(error => {
        console.error('Đăng nhập thất bại:', error);
      });
    }

    loginWithFacebook(): void {
      this.oAthService.signIn(FacebookLoginProvider.PROVIDER_ID).then((userData) => {
        this.user = userData;
        console.log('Facebook User:', this.user);
      }).catch((error) => {
        console.error('Facebook login error:', error);
      });
    }

    

    SaveLocalStorage(data: any) {
      localStorage.setItem("accessToken", data.token.accessToken);
      localStorage.setItem("userName",data.userName);
      localStorage.setItem("fullName",data.fullName);
      localStorage.setItem('userRoles', JSON.stringify(data.roleList));
      localStorage.setItem("confirmEmail", data.confirmEmail);
      localStorage.setItem("userId", data.userId);
      this.authService.setUserName(data.userName);
    }

    ActiveMail(data: any) {
      localStorage.setItem("confirmEmail", data.confirmEmail);
      if(data.confirmEmail == true) {
       this.CheckRoleUser();
      } else {
        this.router.navigate(['/confirmEmail'], { queryParams: { Id: data.userId } });
      }
    }

    CheckRoleUser() {
      if(this.authService.GetRoleUser() == roles.Manager){
        this.router.navigate(["/admin"]);
      }
      else {
        this.router.navigate(["/home"]);
      }
    }

    // Sự kiện khi người dùng nhập ký tự
  onPasswordInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.passwordEntered = input.value.length > 0;  // Kiểm tra xem có ký tự nhập vào hay không
  }

    togglePasswordVisibility() {
      this.passwordVisible = !this.passwordVisible;
      this.passwordFieldType = this.passwordVisible ? 'text' : 'password';
    }

    checkWithToken() {
      this.authService.checkHeaderWithToken().subscribe(
        (response) => {
          console.log('✅ Phản hồi từ server:', response);
          alert(`Phản hồi từ server: ${response.message}`);
        },
      );
    }

    

    onSubmit() {
      this.errorMessage = null;
      if (this.loginForm.valid) {
        const loginData = {
          username: this.loginForm.value.username,
          password: this.loginForm.value.password,
          remember: this.loginForm.value.remember
        };
        console.log(loginData);

        this.authService.PostLogin(loginData).subscribe(
          (data) => {
            console.log(data);
            this.loading = false;
            if (data.success == false) {
              setTimeout(() => {
                this.loading = true;
                this.errorMessage = data.message;
              }, 1000);
              
            } else {
                setTimeout(() => {
                  this.loading = true;
                  this.SaveLocalStorage(data);        
                  this.ActiveMail(data);              // Kiểm tra xác nhận mail chưa
                }, 2000);
            }
          }
        );
      } else {
        console.log('Form is invalid');
        this.errorMessage = 'Please fill in all required fields.';
      }
    }

    loginWithFB(): void {
      FB.login((response: any) => {
        if (response.authResponse) {
          console.log('Đăng nhập Facebook thành công', response);
          
          this.user = response.authResponse;
        } else {
          console.error('Đăng nhập Facebook thất bại', response);
        }
      },);
    }

    

    

    ngOnInit(): void {
      if(this.platformService.isBrowser()) {
        this.initializeGoogleLogin();
      }
    }
}
