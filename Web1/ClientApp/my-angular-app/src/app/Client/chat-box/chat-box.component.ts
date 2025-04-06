import { Component } from '@angular/core';

@Component({
  selector: 'app-chat-box',
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css'
})

export class ChatBoxComponent {

  isOpen = false;
  isOpenWidget = true;  // Trạng thái của toàn bộ widget (ẩn hoặc hiện)
  newMessage = '';
  messages = [
    { from: 'admin', text: 'Chào bạn, tôi có thể giúp gì cho bạn?', timestamp: new Date('2025-04-05T08:30:00') },
    { from: 'user', text: 'Chào admin, tôi gặp vấn đề về tài khoản.', timestamp: new Date('2025-04-05T08:31:00') },
    { from: 'admin', text: 'Vui lòng cung cấp thêm thông tin.', timestamp: new Date('2025-04-05T08:32:00') }
  ];

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  closeChat(event: Event) {
    event.stopPropagation();  // Ngừng sự kiện để không kích hoạt toggleChat
    this.isOpenWidget = false; // Ẩn toàn bộ widget chat
  }

  sendMessage() {
    if (this.newMessage.trim()) {
      const newMessage = {
        from: 'user',  // Đảm bảo xác định ai gửi
        text: this.newMessage,
        timestamp: new Date()  // Thêm thời gian hiện tại
      };
      this.messages.push(newMessage);
      this.newMessage = '';  // Xóa nội dung input sau khi gửi
    }
  }
}
