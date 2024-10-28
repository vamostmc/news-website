import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { jwtDecode } from 'jwt-decode';
import { roles } from '../models/role';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
    constructor(private form: FormBuilder, 
                private http: HttpClient,
                private router: Router,
                private authService: AuthenService) {}

    errorMessage: string | null = null;

    loginForm = this.form.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      rememberMe: [false]
    });

    onSubmit() {
      this.errorMessage = null;
      if (this.loginForm.valid) {
        const loginData = {
          username: this.loginForm.value.username,
          password: this.loginForm.value.password
        };
  
        // Gửi yêu cầu POST đến API với responseType là 'text'
        this.http.post('https://localhost:7233/api/Account/Log-In-JWT', loginData, {  responseType: 'text' })
          .subscribe({
            next: (response: any) => {
              // Xử lý phản hồi
              console.log('Response from API:', response);
              if (response.includes("Error")) {
                // Nếu phản hồi có chứa "Error", hãy thông báo cho người dùng
                console.error('Login failed:', response);
                this.errorMessage = "Login failed";
              } else {
                // Xử lý khi đăng nhập thành công
                console.log('Login successful:', response);
                
                localStorage.setItem("keyUser", response);
                localStorage.setItem("userName",loginData.username ?? '');
                this.authService.setUserName(loginData.username ?? '');
                if(this.authService.GetRoleUser() == roles.Manager){
                  
                  this.router.navigate(["/admin"]);
                }
                else {
                  this.router.navigate([""]);
                }
              }
            },
            error: (error) => {
              console.error('Login failed with error:', error);
              this.errorMessage = 'An error occurred while logging in.';
            }
          });
      } else {
        console.log('Form is invalid');
        this.errorMessage = 'Please fill in all required fields.';
      }
    }

    ngOnInit(): void {
      
    }
}
