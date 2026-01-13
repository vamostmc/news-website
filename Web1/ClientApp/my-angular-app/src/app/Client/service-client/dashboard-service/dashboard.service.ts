import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { NotificationModel } from '../../models/notification';
import { CustomerService } from '../customer-service/customer.service';
import { TinTucService } from '../tintuc-service/tin-tuc.service';
import { User } from '../../models/user';
import { NotifyComment } from '../../models/notifyComment';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = 'https://localhost:7233/api/Notification/GetNotify';
  private PostUrl = 'https://localhost:7233/api/Notification/AddNotificationTinTuc';
  private PostCommentUrl = 'https://localhost:7233/api/Notification/AddNotificationBinhLuan';


  getNotifyTinTuc(): Observable<Notification[]> {
    const url = `${this.apiUrl}/1`;
    return this.http.get<Notification[]>(url);
  }

  getNotifyBinhLuan(): Observable<Notification[]> {
    const url = `${this.apiUrl}/2`;
    return this.http.get<Notification[]>(url);
  }

  AddNotifyTinTuc(newData: Notification): Observable<Notification> {
    return this.http.post<Notification>(this.PostUrl,newData);
  }

  AddNotifyBinhLuan(newData: NotifyComment): Observable<NotifyComment> {
    return this.http.post<NotifyComment>(this.PostCommentUrl,newData);
  }
  
//   NotificationUpdateTinTuc(tieude: string) {
//     //Thêm hành động thêm bài viết mới 
//     let newNotification: Notification = {
//       id: 0,                                // ID của thông báo
//       message: "Sửa thông tin bài viết: " + tieude, // Nội dung thông báo
//       createdAt: new Date().toISOString(),  // Thời gian tạo thông báo (ISO 8601 format)
//       isRead: false,
//       typeId: 1                        // Trạng thái đã đọc hay chưa
                          
//     };
//     console.log(`data bài viết truyền là: ${JSON.stringify(newNotification)}`);
//     this.AddNotifyTinTuc(newNotification).subscribe(
//       (response) => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       (error) => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
//   } 

//   NotificationRemoveTinTuc(tieude: string) {
//     // Thêm hành động thêm bài viết mới 
//     let newNotification: Notification = {
//       id: 0,                                // ID của thông báo
//       message: "Xóa bài viết: " + tieude,              // Nội dung thông báo
//       createdAt: new Date().toISOString(),  // Thời gian tạo thông báo (ISO 8601 format)
//       isRead: false,                        // Trạng thái đã đọc hay chưa
//       typeId: 1                        // ID của tin tức liên quan
//     };

//     // Gọi AddNoti và trả về Observable
//     this.AddNotifyTinTuc(newNotification).subscribe(
//       response => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       error => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
    
// }


// NotificationAddTinTuc(tieude: string) {

//     //Thêm hành động thêm bài viết mới 
//     let newNotification: Notification = {
//       id: 0,                                // ID của thông báo
//       message: "Thêm mới bài viết: " + tieude, // Nội dung thông báo
//       createdAt: new Date().toISOString(),  // Thời gian tạo thông báo (ISO 8601 format)
//       isRead: false,                       // Trạng thái đã đọc hay chưa
//       typeId: 1                         // ID của tin tức liên quan
//     };
//     this.AddNotifyTinTuc(newNotification).subscribe(
//       (response) => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       (error) => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
//   } 

//   NotifyUpdateBinhLuan(UserId: string, TinTucid: number) {
//     //Thêm hành động thêm bài viết mới 
//     let newNotification: NotifyComment = {
//       tinTucId: TinTucid,
//       userId: UserId,
//       action: "Edit"
//     };
//     this.AddNotifyBinhLuan(newNotification).subscribe(
//       (response) => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       (error) => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
//   } 

//   NotifyRemoveBinhLuan(UserId: string, TinTucid: number) {
//     // Thêm hành động thêm bài viết mới 
//     let newNotification: NotifyComment = {
//       tinTucId: TinTucid,
//       userId: UserId,
//       action: "Delete"
//     };
//     this.AddNotifyBinhLuan(newNotification).subscribe(
//       (response) => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       (error) => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
// }


//   NotifyAddBinhLuan(UserId: string, TinTucid: number) {
//     //Thêm hành động thêm bài viết mới 
//     let newNotification: NotifyComment = {
//       tinTucId: TinTucid,
//       userId: UserId,
//       action: "Add"
//     };
//     this.AddNotifyBinhLuan(newNotification).subscribe(
//       (response) => {
//         console.log("Thông báo đã được gửi lên API:", response);
//       },
//       (error) => {
//         console.error("Có lỗi khi gửi thông báo lên API:", error);
//       }
//     );
//   } 
  
  constructor(private http: HttpClient, 
              private customerservice: CustomerService,
              private tintucservice: TinTucService
  ) { }
}
