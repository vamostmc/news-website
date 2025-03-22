import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthenService } from '../../service-client/authen-service/authen.service';
import { Router } from '@angular/router';
import { error } from 'console';
import { emailValidator } from '../../customer-validator/email-validator';
import { passwordValidator } from '../../customer-validator/password-validator';
import { confirmPasswordValidator } from '../../customer-validator/confirmpassword-validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm !: FormGroup;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, 
              private http: HttpClient, 
              private router: Router,
              private _authenservice: AuthenService) {}

  // Khởi tạo form đăng kí
  FormInit() {
    this.registerForm = this.fb.group({
      id: [0],
      userName: ['', Validators.required],       
      dateUser: [new Date(), Validators.required], 
      address: ['', Validators.required],        
      fullName: ['', Validators.required],                  
      email: ['', [Validators.required, emailValidator()]],
      password: ['', [Validators.required, passwordValidator()]],      
      confirmPassword: ['', Validators.required]
    },
    { validators: confirmPasswordValidator('password', 'confirmPassword') }
  );
    this.errorMessage = null;
  };

    
  ngOnInit(): void {
    this.FormInit();
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this._authenservice.PostRegister(this.registerForm).subscribe({
        next: (response: { succeeded: boolean; errors: string[] }) => {
          if (response.succeeded) {
            // Đăng ký thành công
            
            this.errorMessage = null;
            this.registerForm.reset(); // Reset form nếu cần
            this.router.navigate(['/success'], { replaceUrl: true }).then(() => {
              setTimeout(() => {
                this.router.navigate(['/login'], { replaceUrl: true });
              }, 3000);
            });
          } else {
            // Đăng ký thất bại và hiển thị lỗi
            this.errorMessage = response.errors.length > 0 
              ? response.errors.join(', ') // Hiển thị lỗi nếu có
              : 'Đăng ký không thành công. Vui lòng thử lại.';
          }
        },
        error: (error) => {
          // Lỗi kết nối hoặc lỗi từ server
          this.errorMessage = 'Có lỗi xảy ra khi kết nối với server.';
          
        }
      });
    } else {
      this.errorMessage = 'Vui lòng điền đầy đủ thông tin.';
    }
  }
  
  
}
