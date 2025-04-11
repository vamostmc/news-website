export interface Conversation {
   id: number,
   userId: string,
   isActive: boolean,
   userName?: string,
   lastMessage?: string
}