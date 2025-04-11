import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Conversation } from '../../models/conversation';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';
import { MessageModel } from '../../models/message';

@Injectable({
  providedIn: 'root'
})
export class ChatboxService {

  constructor(private http: HttpClient) { }

  CreateConversation(Data: Conversation): Observable<Conversation> {
    return this.http.post<Conversation>(API_ENDPOINTS.chatbox.CreateConversation, Data);
  }

  CheckConversation(id: string): Observable<any> {
    return this.http.get<any>(API_ENDPOINTS.chatbox.CheckCoversation(id));
  }

  GetMessages(id: string, limit: number, beforeTime?: string): Observable<MessageModel[]> {
    let params = new HttpParams().set('limit', limit.toString());
    if (beforeTime) {
      params = params.set('beforeTime', beforeTime);
    }
  
    return this.http.get<MessageModel[]>(API_ENDPOINTS.chatbox.GetMessages(id), { params });
  }

  GetAllCoversation(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(API_ENDPOINTS.chatbox.GetAllConversation);
  }
}
