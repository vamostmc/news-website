export interface Notification {
    id: number;          // ID của thông báo
    message: string;     // Nội dung thông báo
    timestamp: string;   // Thời gian tạo thông báo (ISO 8601 format)
    isRead: boolean;     // Trạng thái đã đọc hay chưa
    typeId?: number;
}