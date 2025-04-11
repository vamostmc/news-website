export interface MessageModel 
{
    Id: number,
    conversationId: number,
    senderId: string,
    receiverId: string,
    text: string,
    sentAt: Date,
    isRead: boolean,
    status: string
}