export interface Comment {
  binhluanId: number;
  tintucId: number;
  userId: string;
  ngayGioBinhLuan: Date;
  noiDung: string;
  tieuDeTinTuc?: string;
  userName?: string;
  trangThai?: boolean;
  parentId?: number;
  likes?: number;
  replies?: Comment[];
}