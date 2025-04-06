import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { AuthenService } from '../../service-client/authen-service/authen.service';
import { TinTucService } from '../../service-client/tintuc-service/tin-tuc.service';
import { DanhmucService } from '../../service-client/danhmuc-service/danhmuc.service';
import { ActivatedRoute } from '@angular/router';
import { NotificationComponent } from '../../notification/notification.component';
import { NotificationService } from '../../service-client/notification-service/notification.service';
import { SignalRService } from '../../service-client/signalR-service/signal-r.service';

@Component({
  selector: 'app-navbar-client',
  templateUrl: './navbar-client.component.html',
  styleUrl: './navbar-client.component.css'
})
export class NavbarClientComponent implements OnInit {
  @ViewChild(NotificationComponent) notificationComponent!: NotificationComponent;

  today: Date = new Date();
  dayOfWeek: string = '';
  formattedDate: string = '';
  fullName: string | null = null;
  userId: string | null = null;
  totalNotify !: number;
  isRinging = false;

  constructor(
    private authen: AuthenService,
    private tintucService: TinTucService,
    private danhmucservice: DanhmucService,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private signalRService: SignalRService
  ) {

  }

  ngOnInit(): void {
    this.signalRService.startConnection();
    this.getTodayInfo();
    this.fullName = localStorage.getItem('fullName');
    this.InitTotalUnread();

    this.signalRService.totalNotifyChange
    .subscribe((increment) => {
      // Tăng số lượng thông báo lên 1 mỗi khi nhận được sự kiện
      this.totalNotify += increment;
      this.toggleBellRingEffect();
    });
  }

  InitTotalUnread() {
    this.userId = localStorage.getItem('userId') || 'defaultUserId';
    this.notificationService.getUnreadCount(this.userId).subscribe(
    (data) => {
      this.totalNotify = data;
      console.log("Tổng");
      console.log(this.totalNotify);
      this.toggleBellRingEffect();
    })
  }


  // Hàm kiểm tra và thêm lớp ring vào chuông
  toggleBellRingEffect() {
    this.isRinging = this.totalNotify > 0;
  }

  toggleNotifications() {
    this.notificationComponent.togglePanel();
  }

  onTotalNotifyChange(count: number) {
    this.totalNotify = count; 
    if(this.totalNotify == 0) {
      this.isRinging = false;
    }
    console.log("Tổng truyền lên cha nhận là : ",this.totalNotify);
  }

  getAllNotify() {
    this.userId = localStorage.getItem('userId') || 'defaultUserId';
    this.notificationComponent.GetDataNotification(this.userId);
  }


  getTodayInfo() {
    const daysOfWeek = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
    this.dayOfWeek = daysOfWeek[this.today.getDay()];

    const date = this.today.getDate();
    const month = this.today.getMonth() + 1;
    const year = this.today.getFullYear();
    
    this.formattedDate = `${date}/${month}/${year}`;
  }
}
