export interface Comment {
  binhluanId: number;
  tintucId: number;
  userId: string;
  ngayGioBinhLuan: string;
  noiDung: string;
  tieuDeTinTuc?: string;
  userName?: string;
  trangThai?: boolean;
  parentId?: number;
  likes?: number;
  replyToUserId? : string;
  userReplyName?: string;
  replies?: Comment[];
}