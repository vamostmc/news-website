import { EventEmitter, Injectable, Signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment.development.js';
import { Observable, Subject } from 'rxjs';
import { MessageModel } from '../../models/message.js';
import { PlatformService } from '../platform-service/platform.service.js';
import { AuthenService } from '../authen-service/authen.service.js';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection!: signalR.HubConnection;
  public totalNotifyChange = new EventEmitter<number>(); 
  public newMessageReceived = new EventEmitter<MessageModel>();

  constructor(private authenService: AuthenService) {
    
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

      this.hubConnection.on('ReceiveChatMessage', (message) => {
        console.log('ChatMessage Received:', message);
        this.newMessageReceived.emit(message);
      });

  }

  sendMessageToUser(messageUser: MessageModel): void {
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection
        .invoke('SendMessagefromClient', messageUser)
        .then(() => console.log('Message sent to server successfully'))
        .catch(err => console.error('Error sending message:', err));
    } else {
      console.warn('Cannot send message. SignalR connection is not established yet.');
    }
  }

}
