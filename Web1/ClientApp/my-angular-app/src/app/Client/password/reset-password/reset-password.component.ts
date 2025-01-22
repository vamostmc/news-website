import { Component, input, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { PasswordService } from '../../service-client/password-service/password.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { confirmPasswordValidator } from '../../customer-validator/confirmpassword-validator';
import { passwordValidator } from '../../customer-validator/password-validator';
import { resetPassword } from '../../models/reset-password';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  
  constructor(private route: ActivatedRoute,
              private passwordService: PasswordService,
              private router: Router,
              private fb: FormBuilder
  ) {}

  errorMessage: string | null = null;
  UpdateForm !: FormGroup;
  Id : string | null = null;
  Token : string | null = null;
  loading : boolean = false;

  FormInit() {
    this.UpdateForm = this.fb.group({
      password: ['', [Validators.required, passwordValidator()]],      
      confirmPassword: ['', Validators.required]
    },
    { validators: confirmPasswordValidator('password', 'confirmPassword') }
  );
    this.errorMessage = null;
  };

  // Lấy IdUser từ đường dẫn
  getUserId() {
    this.Id = this.route.snapshot.paramMap.get('id');
    if (!this.Id) {
      console.error('Không tìm thấy tham số id trong URL');
    }
  }

  // Lấy Token từ đường dẫn
  getResetToken() {
    this.Token = this.route.snapshot.paramMap.get('resetToken');
    if (!this.Token) {
      console.error('Không tìm thấy tham số id trong URL');
    }
  }

  onSubmit() {
    if(this.UpdateForm.valid) {
      console.log(this.UpdateForm.value);
      const FormUser: resetPassword = {
        newPassword: this.UpdateForm.value.password,
        userId: this.Id || '0',
        resetToken: this.Token || '0'
      }

      this.passwordService.ResetPassword(FormUser).subscribe(
        (data) => {
          if(data.success == true) {
            this.loading = true;
            setTimeout(() => {
              this.router.navigate(['/success']);
              sessionStorage.removeItem("ResetToken");
            }, 1000); 
          }
          else {
            this.loading = true;
            setTimeout(() => {
              this.loading = false;
              this.errorMessage = data.message;
            }, 1000);
          }
        }
      );
    } 
  }

  ngOnInit(): void {
    this.FormInit();
    this.getUserId();  
    this.getResetToken();
  }
}

  
