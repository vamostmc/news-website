import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errorMessage: string | null;

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router) {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      userName: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
    this.errorMessage = null;
  }

  ngOnInit(): void {
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      // Xử lý đăng ký ở đây
      const data = this.registerForm.value;

      this.http.post("https://localhost:7233/api/Account/Sign-Up", data).subscribe({
        next: (response) => {
          if(response == true) {
            console.log('Registration successful:', response);
            // Handle successful registration
          }
          else {
            this.errorMessage = 'Error Password';
          }

        },
        error: (error) => {
            console.error('Registration failed:', error); // Log lỗi
            this.errorMessage = 'Error Password 1';
        }
          });
      } else {
          this.errorMessage = 'Error Password 2';
      }
  }
}
