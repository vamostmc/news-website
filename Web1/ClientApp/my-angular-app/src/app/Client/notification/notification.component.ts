import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { NotificationService } from '../service-client/notification-service/notification.service';
import { NotificationModel } from '../models/notification';
import { SignalRService } from '../service-client/signalR-service/signal-r.service';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.css'
})
export class NotificationComponent implements OnInit {

  notifications: NotificationModel[] = [];
  selectAllChecked = false;
  totalNotify: number = 0; // Biến lưu số lượng chưa đọc
  isPanelVisible = false;
  receivedMessage !: string;
  @Output() totalNotifyChange = new EventEmitter<number>(); // Sự kiện truyền lên cha

  constructor(private notificationService: NotificationService,
              private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    
  }

  togglePanel() {
    this.isPanelVisible = !this.isPanelVisible;
    console.log(this.isPanelVisible);
  }

  GetDataNotification(id: string) {
    this.notificationService.GetNotify(id).subscribe(
      (data) => {
        console.log("Thông báo từ server");
        console.log(data);
        this.notifications = data.sort((a, b) => {
          // Ưu tiên chưa đọc
          if (a.isRead !== b.isRead) {
            return a.isRead ? 1 : -1;
          }
        
          // Nếu cùng trạng thái isRead, so sánh thời gian (mới nhất trước)
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        });
      }
    );
  }

  DeleteDataNotification(id: number) {
    console.log(id);
    this.notificationService.deleteNotify(id).subscribe(
      (data) => {
        if(data.success == true) {
          this.notifications = this.notifications.filter(notify => notify.id !== id);
          this.updateTotalNotify();
        }
      }
    );
  }

  getTimeAgo(timestamp: string): string {
    const now = new Date();
    const timeDiff = now.getTime() - new Date(timestamp).getTime();
    
    const seconds = Math.floor(timeDiff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    
    if (seconds < 60) {
      return `${seconds} giây trước`;
    } else if (minutes < 60) {
      return `${minutes} phút trước`;
    } else if (hours < 24) {
      return `${hours} giờ trước`;
    } else {
      return `${days} ngày trước`;
    }
  }

  updateReadStatus() {
    var userId = localStorage.getItem('userId') || 'defaultUserId';
    this.notificationService.updateStatusReadAll(userId).subscribe(
      (data) => {
        if(data.success == true) {
          this.notifications.forEach(notify => {
            if (notify.isRead == false) { 
              notify.isRead = true;
            }
          });
          this.updateTotalNotify();
        }
      }
    );
  }

  // Hàm mới để đánh dấu 1 tin là đã đọc
  markAsRead(id: number) { 
    this.notificationService.updateStatusReadId(id).subscribe(
      (data) => {
        if(data.success == true) {
          this.notifications.forEach(notify => {
            if (notify.id == id && notify.isRead == false) { 
              notify.isRead = true;
            }
          });
          this.updateTotalNotify();
        }
      }
    );
    console.log("Cập nhật noti mới: ",this.notifications); 
  }

  private updateTotalNotify() {
    this.totalNotify = this.notifications.filter(notify => !notify.isRead).length;
    this.totalNotifyChange.emit(this.totalNotify); 
  }

  sendMessage() {
    // const userId = "user123"; // Thay userId bằng user đang đăng nhập
    // const message = "🔔 Đây là tin nhắn thử nghiệm từ client!";
    // this.signalRService.sendMessageToUser(userId, message);
  }

  

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const panel = document.querySelector('.notification-panel') as HTMLElement;
    const target = event.target as HTMLElement;

    // Chỉ tắt bảng nếu nhấp ra ngoài panel và không phải icon chuông
    if (this.isPanelVisible && panel && !panel.contains(target) && !target.closest('.circle-bell')) {
      this.isPanelVisible = false;
    }
  }
}
