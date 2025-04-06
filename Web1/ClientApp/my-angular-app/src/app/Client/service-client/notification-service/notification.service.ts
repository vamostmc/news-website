import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';
import { Observable } from 'rxjs';
import { NotificationModel } from '../../models/notification.js';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private http: HttpClient) { }

  getUnreadCount(userId: string): Observable<number> {
    return this.http.get<number>(API_ENDPOINTS.notification.TotalNotify(userId));
  }

  GetNotify(userId: string): Observable<NotificationModel[]> {
    return this.http.get<NotificationModel[]>(API_ENDPOINTS.notification.getNotify(userId));
  }

  deleteNotify(Id: number): Observable<any> {
    return this.http.delete<any>(API_ENDPOINTS.notification.DeleteNotify(Id));
  }

  updateStatusReadId(id: number): Observable<any> {
    return this.http.put<any>(API_ENDPOINTS.notification.UpdateStatusId(id), true);
  }

  updateStatusReadAll(id: string): Observable<any> {
    return this.http.put<any>(API_ENDPOINTS.notification.UpdateStatusAll(id), true);
  }
}
