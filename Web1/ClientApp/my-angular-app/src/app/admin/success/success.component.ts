import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';


@Component({
  selector: 'app-success',
  templateUrl: './success.component.html',
  styleUrl: './success.component.css'
})
export class SuccessComponent implements OnInit {
  countdown: number = 3; // Thời gian đếm ngược bắt đầu từ 5 giây
  countdownInterval: any; // Biến để lưu giữ interval
  showNotification: boolean = false;
  @Input() typeNotify : boolean = true;

  @Input() messageNotify: string[] = [];

  

  constructor(private router: Router, private location: Location) {} // Inject Router

  ngOnInit(): void {
    if(this.typeNotify == true) {
      this.startCountdown();
    }
     // Bắt đầu đếm ngược khi component được khởi tạo

    
  }



  startCountdown() {
    this.countdownInterval = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        this.goBack(); // Tự động quay lại khi đếm xong
      }
    }, 1000); // Giảm bộ đếm mỗi giây
  }

  closeEditModal() {
    const modalElement = document.getElementById('editPostModal');
    if (modalElement) {
      const modal = (window as any).bootstrap.Modal.getInstance(modalElement);
      if (modal) {
        modal.hide(); // Đóng modal
      }
    }
  }
  
  goBack() {
    clearInterval(this.countdownInterval);
    this.closeEditModal();
    this.showNotification = false;
    window.history.back();
    

  }
}
