import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ChatBoxComponent } from '../../Client/chat-box/chat-box.component';
import { ChatboxService } from '../../Client/service-client/chatbox-service/chatbox.service';
import { Conversation } from '../../Client/models/conversation';
import { MessageModel } from '../../Client/models/message';
import { SignalRService } from '../../Client/service-client/signalR-service/signal-r.service';

@Component({
  selector: 'app-manage-message',
  templateUrl: './manage-message.component.html',
  styleUrl: './manage-message.component.css'
})
export class ManageMessageComponent implements OnInit, AfterViewInit {

  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  currentUserId = 'Manager';
  newMessage = '';
  limit = 10;

  conversations: Conversation[] = [];
  selectedConversation: Conversation | null = null;
  isLoadingOlderMessages = false;

  rawMessages: MessageModel[] = [];
  messages: {
    type: 'message' | 'time',
    from?: string,
    text?: string,
    timestamp: Date
  }[] = [];

  constructor(
    private chatboxService: ChatboxService,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    this.getConvers();

    this.signalRService.newMessageReceived.subscribe((message: MessageModel) => {
      if (!this.selectedConversation || message.conversationId !== this.selectedConversation.id) return;

      this.rawMessages.push(message);

      const lastDate = this.getLastDateFromMessages();
      const msgDate = new Date(message.sentAt).toLocaleDateString();

      if (msgDate !== lastDate) {
        this.messages.push({ type: 'time', timestamp: new Date(message.sentAt) });
      }

      this.messages.push({
        type: 'message',
        from: message.senderId === this.currentUserId ? 'admin' : 'user',
        text: message.text,
        timestamp: new Date(message.sentAt)
      });

      this.scrollToBottom();
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.scrollToBottom();
    }, 200);
  }

  scrollToBottom(): void {
    try {
      setTimeout(() => {
        this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
      }, 100);
    } catch (err) {
      console.error('Scroll error:', err);
    }
  }

  onScroll(): void {
    const el = this.scrollContainer?.nativeElement;
    if (!el) return;
  
    const scrollTop = el.scrollTop;
  
    if (scrollTop <= 2 && !this.isLoadingOlderMessages) {
      this.isLoadingOlderMessages = true;
      setTimeout(() => {
        this.loadOlderMessages();
      }, 1500);
    }
  }

  loadOlderMessages() {
    this.isLoadingOlderMessages = true;
  
    // Lấy thời điểm của tin đầu tiên hiện tại
    const firstMessage = this.rawMessages[0];
    console.log("data tin nhắn đầu tiên: ",firstMessage);
    const beforeTime = firstMessage?.sentAt?.toString();
    console.log("thời gian tin nhắn đầu tiên: ",beforeTime);
  
    const el = this.scrollContainer.nativeElement;
    const previousHeight = el.scrollHeight;
  
    this.chatboxService.GetMessages(this.selectedConversation!.userId, this.limit, beforeTime).subscribe((olderMessages: MessageModel[]) => {
      // Sắp xếp tăng dần để hiển thị đúng thứ tự
      olderMessages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
  
      // Ghép vào đầu mảng gốc
      this.rawMessages = [...olderMessages, ...this.rawMessages];
      this.messages = this.formatMessagesWithTimestamps(this.rawMessages, this.currentUserId);
  
      this.isLoadingOlderMessages = false;
  
      // Giữ đúng vị trí cuộn
      setTimeout(() => {
        const newHeight = el.scrollHeight;
        el.scrollTop = newHeight - previousHeight;
      });
    });
  }

  getConvers(): void {
    this.chatboxService.GetAllCoversation().subscribe((data) => {
      this.conversations = data;
    });
  }

  selectConversation(conversation: Conversation): void {
    this.selectedConversation = conversation;
    const limit = 10;

    this.chatboxService.GetMessages(conversation.userId,limit).subscribe((messages: MessageModel[]) => {
      const sorted = messages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
      this.rawMessages = sorted;
      this.messages = this.formatMessagesWithTimestamps(sorted, this.currentUserId);

      setTimeout(() => {
        this.scrollToBottom();
      }, 200);
    });
  }

  formatMessagesWithTimestamps(messages: MessageModel[], currentUserId: string) {
    let lastDate = '';
    return messages.flatMap(msg => {
      const msgDate = new Date(msg.sentAt).toLocaleDateString();
      const entries: any[] = [];

      if (msgDate !== lastDate) {
        entries.push({ type: 'time', timestamp: new Date(msg.sentAt) });
        lastDate = msgDate;
      }

      entries.push({
        type: 'message',
        from: msg.senderId === currentUserId ? 'admin' : 'user',
        text: msg.text,
        timestamp: new Date(msg.sentAt)
      });

      return entries;
    });
  }

  getLastDateFromMessages(): string {
    for (let i = this.messages.length - 1; i >= 0; i--) {
      if (this.messages[i].type === 'time') {
        return new Date(this.messages[i].timestamp).toLocaleDateString();
      }
    }
    return '';
  }

  sendMessage(): void {
    if (!this.newMessage.trim() || !this.selectedConversation) return;

    const now = new Date();
    const today = now.toLocaleDateString();
    const lastDate = this.getLastDateFromMessages();

    const newMsg: MessageModel = {
      Id: 0,
      conversationId: this.selectedConversation.id,
      senderId: this.currentUserId,
      receiverId: this.selectedConversation.userId,
      text: this.newMessage,
      sentAt: now,
      isRead: false,
      status: ''
    };

    this.signalRService.sendMessageToUser(newMsg);

    if (today !== lastDate) {
      this.messages.push({ type: 'time', timestamp: now });
    }

    this.messages.push({
      type: 'message',
      from: 'admin',
      text: this.newMessage,
      timestamp: now
    });

    this.newMessage = '';
    this.scrollToBottom();
  }

  getVietnameseDayName(date: Date): string {
    const days = [
      'Chủ nhật', 'Thứ hai', 'Thứ ba',
      'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy'
    ];
    return days[new Date(date).getDay()];
  }
}
