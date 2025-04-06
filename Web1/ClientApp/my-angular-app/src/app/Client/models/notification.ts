export interface NotificationModel {
    id: number;
    message: string;
    createdAt: string;
    isRead: boolean;
    typeId: number;
    targetId: string;
    senderId: string;
    isSystem: boolean;
}