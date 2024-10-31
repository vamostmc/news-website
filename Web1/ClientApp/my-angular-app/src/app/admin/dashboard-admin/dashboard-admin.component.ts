import { Component, OnInit } from '@angular/core';
import { Tintuc } from '../../Client/models/tintuc';
import { TinTucService } from '../../Client/service-client/tintuc-service/tin-tuc.service';
import { CustomerService } from '../../Client/service-client/customer-service/customer.service';
import { CommentService } from '../../Client/service-client/comment-service/comment.service';
import { DashboardService } from '../../Client/service-client/dashboard-service/dashboard.service';
import { Notification } from '../../Client/models/notification';
import { Comment } from '../../Client/models/comment';

@Component({
  selector: 'app-dashboard-admin',
  templateUrl: './dashboard-admin.component.html',
  styleUrl: './dashboard-admin.component.css'
})
export class DashboardAdminComponent implements OnInit {
    constructor(private tintucservice: TinTucService, 
                private customerservice: CustomerService,
                private commentservice: CommentService,
                private dashboardservice: DashboardService) {}
    
    tintucs: Tintuc[] = []
    selectedTintuc: Tintuc | null = null;
    totalCustomer : number | null = null;
    comments : Comment[] = [];
    notificationsTinTuc: Notification[] = [];
    notificationsBinhLuan: Notification[] = [];

    itemsPerPage: number = 4;
    pTinTuc: number = 1; // Biến để lưu số trang hiện tại
    pBinhLuan: number = 1;
    onPageChangeTinTuc(page: number) {
      this.pTinTuc = page;
    }

    onPageChangeBinhLuan(page: number) {
      this.pBinhLuan = page;
    }

    get paginatedTinTuc() {
      const start = (this.pTinTuc - 1) * this.itemsPerPage;
      return this.notificationsTinTuc.slice(start, start + this.itemsPerPage);
    }
  
    get paginatedBinhLuan() {
      const start = (this.pBinhLuan - 1) * this.itemsPerPage;
      return this.notificationsBinhLuan.slice(start, start + this.itemsPerPage);
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

    ngOnInit(): void {
      this.tintucservice.getTintuc().subscribe(
        (data) => this.tintucs = data
      )

      this.customerservice.getTotalCustomer().subscribe(
        (data) => this.totalCustomer = data
      )

      this.commentservice.getAllComment().subscribe(
        (data) => { this.comments = data;}
      )

      this.dashboardservice.getNotifyTinTuc().subscribe(
        (data) => { this.notificationsTinTuc = data.reverse(); console.log(this.notificationsTinTuc);}
      )

      this.dashboardservice.getNotifyBinhLuan().subscribe(
        (data) => { this.notificationsBinhLuan = data.reverse(); console.log(this.notificationsBinhLuan);}
      )
      
    }
}
