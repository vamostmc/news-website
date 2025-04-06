import { EventEmitter, Injectable, Signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment.development.js';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection!: signalR.HubConnection;
  public totalNotifyChange = new EventEmitter<number>(); 

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.API_BASE}/notificationHub`) 
      .withAutomaticReconnect()
      .build();
  }

  private API_BASE = environment.baseUrl;

  startConnection() {
    const token = localStorage.getItem('accessToken') || ''; 

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.API_BASE}/notificationHub`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('Error while starting connection: ' + err));

      this.hubConnection.on('ReceiveNotification', (message) => {
        console.log('Notification Received:', message);
        
        // Phát sự kiện tăng số lượng thông báo lên 1 mỗi khi nhận được thông báo mới
        this.totalNotifyChange.emit(1);  
      });
    
  }

  
}
