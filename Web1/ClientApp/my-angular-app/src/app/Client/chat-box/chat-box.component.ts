import { AfterViewInit, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { ChatboxService } from '../service-client/chatbox-service/chatbox.service';
import { Conversation } from '../models/conversation';
import { SignalRService } from '../service-client/signalR-service/signal-r.service';
import { MessageModel } from '../models/message';
import { PlatformService } from '../service-client/platform-service/platform.service';

@Component({
  selector: 'app-chat-box',
  templateUrl: './chat-box.component.html',
  styleUrls: ['./chat-box.component.css']
})
export class ChatBoxComponent implements OnInit, AfterViewInit {

  isOpen = false;
  isOpenWidget = true;
  hasStarted = false;
  newMessage = '';
  dataConversation!: Conversation;
  isLoadingOlderMessages = false;

  rawMessages: MessageModel[] = [];
  messages: { 
    type: 'message' | 'time', 
    from?: string, 
    text?: string, 
    timestamp: Date 
  }[] = [];

  @ViewChild('scrollContainer', { static: false }) scrollContainer?: ElementRef;
  private wheelListenerAttached = false;

  constructor(
    private chatboxService: ChatboxService, 
    private signalRService: SignalRService,
    private platformService: PlatformService
  ) {}

  private wheelListener!: (e: WheelEvent) => void;

  ngOnInit(): void {
    this.isOpenWidget = false;
    this.checkConverServer();

    this.signalRService.newMessageReceived.subscribe((message: MessageModel) => {
      this.rawMessages.push(message);
      const lastDate = this.getLastDateFromMessages();
      const msgDate = new Date(message.sentAt).toLocaleDateString();

      if (msgDate !== lastDate) {
        this.messages.push({ type: 'time', timestamp: new Date(message.sentAt) });
      }

      this.messages.push({
        type: 'message',
        from: message.senderId === message.receiverId ? 'user' : 'admin',
        text: message.text,
        timestamp: new Date(message.sentAt)
      });

      this.scrollToBottom();
    });
  }

  ngAfterViewInit(): void {
    // Giữ nguyên đoạn scroll sau 200ms nếu đã bắt đầu và đang mở
    setTimeout(() => {
      if (this.isOpen && this.hasStarted) {
        this.scrollToBottom();
      }
    }, 200);
  
    // Đảm bảo phần tử DOM đã tồn tại trước khi thao tác
    setTimeout(() => {
      const el = this.scrollContainer?.nativeElement;
      if (!el || this.wheelListenerAttached) return;
  
      el.addEventListener('wheel', (e: WheelEvent) => {
        const delta = e.deltaY;
        const scrollTop = el.scrollTop;
        const scrollHeight = el.scrollHeight;
        const offsetHeight = el.offsetHeight;
  
        const isAtTop = scrollTop <= 0;
        const isAtBottom = scrollTop + offsetHeight >= scrollHeight;
  
        if ((delta < 0 && isAtTop) || (delta > 0 && isAtBottom)) {
          e.preventDefault();
          e.stopPropagation();
        }
      }, { passive: false });
  
      this.wheelListenerAttached = true;
    }, 300); // delay để đảm bảo DOM được render xong
  }
  
  onScroll(): void {
    const el = this.scrollContainer?.nativeElement;
    if (!el) return;
  
    const scrollTop = el.scrollTop;
  
    if (scrollTop <= 5 && !this.isLoadingOlderMessages) {
      this.isLoadingOlderMessages = true;
      setTimeout(() => {
        this.loadOlderMessages();
      }, 1500);
    }
  }
  
  private scrollToBottom(): void {
    const el = this.scrollContainer?.nativeElement;
    if (!el) return;
  
    setTimeout(() => {
      try {
        el.scrollTop = el.scrollHeight;
      } catch (err) {
        console.error('Scroll error:', err);
      }
    }, 100);
  }

  loadOlderMessages() {
    this.isLoadingOlderMessages = true;
    // let userId = '0';
    // if (this.platformService.isBrowser()) {
    //   userId = localStorage.getItem('userId') || '0';
    // }
    const  userId = localStorage.getItem('userId') || '';
    const limit = 10;
  
    // Lấy thời điểm của tin đầu tiên hiện tại
    const firstMessage = this.rawMessages[0];
    console.log("data tin nhắn đầu tiên: ",firstMessage);
    const beforeTime = firstMessage?.sentAt?.toString();
    console.log("thời gian tin nhắn đầu tiên: ",beforeTime);
  
    const el = this.scrollContainer?.nativeElement;
    if (!el) {
      console.warn('scrollContainer is not yet available when loading older messages');
      this.isLoadingOlderMessages = false;
      return;
    }
    const previousHeight = el.scrollHeight;
  
    this.chatboxService.GetMessages(userId, limit, beforeTime).subscribe((olderMessages: MessageModel[]) => {
      // Sắp xếp tăng dần để hiển thị đúng thứ tự
      olderMessages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
  
      // Ghép vào đầu mảng gốc
      this.rawMessages = [...olderMessages, ...this.rawMessages];
      this.messages = this.formatMessagesWithTimestamps(this.rawMessages, userId);
  
      this.isLoadingOlderMessages = false;
  
      // Giữ đúng vị trí cuộn
      setTimeout(() => {
        const newHeight = el.scrollHeight;
        el.scrollTop = newHeight - previousHeight;
      });
    });
  }
  
  

  checkConverServer() {
    // let userId = '0';
    // if (this.platformService.isBrowser()) {
    //   userId = localStorage.getItem('userId') || '0';
    //   localStorage.removeItem('statusConversation');
    //   localStorage.removeItem('ConversationId');
    // }

    const userId = localStorage.getItem('userId') || '0';
    localStorage.removeItem('statusConversation');
    localStorage.removeItem('ConversationId');

    this.chatboxService.CheckConversation(userId).subscribe((data) => {
      if (this.platformService.isBrowser()) {
        localStorage.setItem('statusConversation', data.success);
      }
      if (!data.success) {
        this.hasStarted = false;
      } else {
        this.hasStarted = true;
        if (this.platformService.isBrowser()) {
          localStorage.setItem('ConversationId', data.message);
        }
        this.loadMessagesFromApi();
      }
    });
  }

  loadMessagesFromApi() {
    // let userId = '0';
    // if (this.platformService.isBrowser()) {
    //   userId = localStorage.getItem('userId') || '0';
    // }
    const userId = localStorage.getItem('userId') || '';
    const limit = 10;
  
    this.chatboxService.GetMessages(userId, limit).subscribe((repo: MessageModel[]) => {
      // Sắp xếp tăng dần theo thời gian
      repo.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
  
      this.rawMessages = repo;
      this.messages = this.formatMessagesWithTimestamps(repo, userId);
  
      if (this.messages.length > 0) {
        this.hasStarted = true;
        this.scrollToBottom();
      }
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
        from: msg.senderId === currentUserId ? 'user' : 'admin',
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

  toggleChat() {
    this.isOpen = !this.isOpen;
    this.isOpenWidget = true;
    setTimeout(() => this.scrollToBottom(), 200);
  }

  closeChat(event: Event) {
    event.stopPropagation();
    this.isOpenWidget = false;
  }

  startChat() {
    this.hasStarted = true;
    // let userId = '0';
    // if (this.platformService.isBrowser()) {
    //   userId = localStorage.getItem('userId') || '0';
    // }
    const userId = localStorage.getItem('userId') || '';
    this.dataConversation = {
      id: 0,
      userId: userId,
      isActive: true
    };

    this.chatboxService.CreateConversation(this.dataConversation).subscribe((repo) => {
      if (this.platformService.isBrowser()) {
        localStorage.setItem("ConversationId", repo.id.toString());
        localStorage.setItem("statusConversation", "true");
      }
    });

    const now = new Date();
    const today = now.toLocaleDateString();
    const lastDate = this.getLastDateFromMessages();

    if (today !== lastDate) {
      this.messages.push({ type: 'time', timestamp: now });
    }

    this.messages.push({
      type: 'message',
      from: 'admin',
      text: 'Chào bạn, tôi có thể giúp gì cho bạn?',
      timestamp: now
    });

    this.scrollToBottom();
  }

  sendMessage() {
    if (!this.newMessage.trim()) return;

    const now = new Date();
    
    const today = now.toLocaleDateString();
    const lastDate = this.getLastDateFromMessages();

    if (today !== lastDate) {
      this.messages.push({ type: 'time', timestamp: now });
    }

    const newMessageUser = {
      type: 'message' as const,
      from: 'user',
      text: this.newMessage,
      timestamp: now
    };

    let currentUserId = '0';
    let conversationId = 0;

    if (this.platformService.isBrowser()) {
      currentUserId = localStorage.getItem('userId') || '';
      conversationId = Number(localStorage.getItem('ConversationId') ?? 0);
    }

    

    

    const messageToSend: MessageModel = {
      Id: 0,
      conversationId: Number(localStorage.getItem('ConversationId') ?? 0),
      senderId: currentUserId,
      receiverId: 'Manager',
      text: this.newMessage,
      sentAt: now,
      isRead: false,
      status: ''
    };

    this.messages.push(newMessageUser);
    this.signalRService.sendMessageToUser(messageToSend);
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
