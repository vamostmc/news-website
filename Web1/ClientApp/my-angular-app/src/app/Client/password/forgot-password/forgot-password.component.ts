import { Component, input, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { ConfirmMailService } from '../../service-client/confirm-mail-service/confirm-mail.service';
import { verifyCode } from '../../models/verifyCode';
import { PasswordService } from '../../service-client/password-service/password.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent implements OnInit {
    // Sử dụng mảng để lưu trữ giá trị của các ô nhập
  code: string[] = ['', '', '', '', '', ''];
  emailUser: string = '';
  IdUser: string = '';
  NameOrEmail: string = '';
  sendedCode: boolean = false;
  loading: boolean = false;
  errorMessage: string | null = null;
  

  // Mảng các trường (chỉ cần sử dụng mảng này để lặp trong HTML)
  fields = [1, 2, 3, 4, 5, 6];

  isDelayActive = false; // Trạng thái delay
  delaySeconds = 60;     // Thời gian delay (60 giây)
  private delayTimer: any;
  countdown = 59;
  interval: any;

  constructor(private route: ActivatedRoute,
              private passwordService: PasswordService,
              private router: Router
  ) {}

  moveFocus(event: any, nextFieldIndex: number) {
    const inputElement = event.target;
    console.log(nextFieldIndex , inputElement.value);

    // Chặn không cho nhập thêm ký tự nếu ô đã có ký tự
    if (inputElement.value.length > 1) {
      // Xóa ký tự cũ nếu người dùng quay lại ô nhập
      inputElement.value = inputElement.value[0];
    }
  
    //Khi nhập 1 ô sẽ tự động nhảy sang ô tiếp theo
    if (inputElement.value.length === 1) {
      const nextField = document.getElementById('code' + (nextFieldIndex + 1));
      if (nextField) {
        nextField.focus();
      }
    }
  }

  submitCode() {
    // Chuyển đổi mỗi số thành chuỗi và lấy ký tự đầu tiên
    const submittedCode = this.code.map(code => code.toString().charAt(0)).join('');
    console.log('Code submitted:', submittedCode);
    const CodeMail : verifyCode = {
      emailUser: this.emailUser,
      code: submittedCode,
    }
    this.passwordService.SendCodeToServer(CodeMail).subscribe(
      (reposive: any) => {
        console.log(reposive);
        if(reposive.success == true) {
          //OTP đúng sẽ trả token về nhằm xác thực để chặn các người khác vào link này
          sessionStorage.setItem("ResetToken",reposive.message);
          this.router.navigate([`/resetPassword/${this.IdUser}/${reposive.message}`]);
          console.log(reposive.message);
        } 
        if(reposive.success == false && reposive.message === "expired") {
          this.errorMessage = "Mã xác nhận đã quá hạn. Vui lòng nhấn gửi lại"
          console.log(this.errorMessage);
        }
        if(reposive.success == false && reposive.message === "invalid") {
          this.errorMessage = "Mã xác nhận không đúng"
          console.log(this.errorMessage);
        }
      }
    );
  }


  resetCode() {
    if (!this.isDelayActive) {
      this.isDelayActive = true;
      this.startCountdown();
      // Thực hiện hành động khác khi nhấn nút reset
      console.log('Reset code action');
      this.sendVerificationCode(this.NameOrEmail);
    }
  }

  startCountdown() {
    this.interval = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        clearInterval(this.interval);
        this.isDelayActive = false; // Kích hoạt lại nút khi đếm ngược xong
        this.countdown = 59; // Reset lại giá trị đếm ngược
      }
    }, 1000);
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.emailUser = params['email']; // Lấy giá trị của email
      console.log('Email:', this.emailUser);
    });
  }

  sendVerificationCode(inputUser: string) {
    if (!inputUser.trim()) {  
      this.errorMessage = 'Vui lòng nhập email hoặc tên người dùng.';
      return;  // Ngừng thực hiện nếu inputUser là khoảng trắng hoặc rỗng
    }
    
      this.loading = true;
      this.errorMessage = '';
      this.passwordService.SendCodeToUser(inputUser).subscribe(
      (data: any) => {
        if(data.success == true) {
          setTimeout(() => {
            this.loading = false;
            this.emailUser = data.message.split('|')[0];
            this.IdUser = data.message.split('|')[1];
            this.sendedCode = true;
          }, 500);
        } else {
          setTimeout(() => {
            this.loading = false;
            this.errorMessage = data.message;
          }, 500);   
        }
      }
      );
    
  }
}
