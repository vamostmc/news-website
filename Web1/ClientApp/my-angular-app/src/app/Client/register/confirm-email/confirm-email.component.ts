import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { ConfirmMailService } from '../../service-client/confirm-mail-service/confirm-mail.service';
import { verifyCode } from '../../models/verifyCode';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent implements OnInit {
  // Sử dụng mảng để lưu trữ giá trị của các ô nhập
  code: string[] = ['', '', '', '', '', ''];
  emailUser: string = '';
  IdUser: string = '';
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
              private confirmEmail: ConfirmMailService,
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
    this.confirmEmail.SendCodetoServer(CodeMail).subscribe(
      (reposive: any) => {
        if(reposive.success == true) {
          this.router.navigate(['']);
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
      this.sendVerificationCode();
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
      this.IdUser = params['Id']; // Lấy giá trị của email
    });
  }

  sendVerificationCode() {
    this.loading = true;
    this.confirmEmail.SendCodeToUser(this.IdUser).subscribe(
      (data: any) => {
        if(data.success == true) {
          setTimeout(() => {
            this.loading = false;
            this.emailUser = data.message;
            this.sendedCode = true;
          }, 500);
        } else {

        }
      }
    );
  }




  
  
  

  
}
