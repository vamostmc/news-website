import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Comment } from '../../models/comment';
import { FormGroup, FormsModule } from '@angular/forms';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  
  getAllComment(): Observable<Comment[]> {
    return this.http.get<Comment[]>(API_ENDPOINTS.comment.getAll);
  }
  
  getIdComment(id: number): Observable<Comment> {
    return this.http.get<Comment>(API_ENDPOINTS.comment.getById(id));
  }
  
  // Lấy bình luận của bài viết chi tiết
  getCommentTinTuc(id: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(API_ENDPOINTS.comment.getByTinTucId(id));
  }
  
  // Post comment
  PostComment(Data: any): Observable<Comment> {
    return this.http.post<Comment>(API_ENDPOINTS.comment.add, Data);
  }
  
  // Put Commment
  PutComment(id: number, Data: any): Observable<Comment> {
    return this.http.put<Comment>(API_ENDPOINTS.comment.update(id), Data);
  }
  
  // Cập nhật status
  StatusComment(id: number, status: boolean): Observable<Comment[]> {
    return this.http.put<Comment[]>(API_ENDPOINTS.comment.updateStatus(id), status);
  }
  
  // Delete Comment
  DeleteComment(id: number): Observable<any> {
    return this.http.delete<any>(API_ENDPOINTS.comment.delete(id));
  }

  //Chuyển dạng FormGroup Post từ bên user sang dạng FormData
  SendFormPost(postForm: FormGroup): FormData {
    const formData = new FormData();

    formData.append("tintucId", postForm.get("tintucId")?.value);
    formData.append("userId", postForm.get("userId")?.value);
    formData.append("parentId", postForm.get("parentId")?.value);
    formData.append("ngayGioBinhLuan", postForm.get("ngayGioBinhLuan")?.value);
    formData.append("noiDung", postForm.get("noiDung")?.value);
    formData.append("userName", postForm.get("userName")?.value);
    formData.append("trangThai", postForm.get("trangThai")?.value);
    formData.append("replyToUserId", postForm.get("replyToUserId")?.value);
    return formData;
  }

  //Chuyển dạng FormGroup Put từ bên user sang FormData
  SendFormEdit(postForm: FormGroup): FormData {
    const formData = new FormData();
    formData.append("binhluanId", postForm.get("binhluanId")?.value);
    formData.append("tintucId", postForm.get("tintucId")?.value);
    formData.append("parentId", postForm.get("parentId")?.value);
    formData.append("userId", postForm.get("userId")?.value);
    formData.append("ngayGioBinhLuan", postForm.get("ngayGioBinhLuan")?.value);
    formData.append("noiDung", postForm.get("noiDung")?.value);
    formData.append("trangThai", postForm.get("trangThai")?.value);
    formData.append("replyToUserId", postForm.get("replyToUserId")?.value);
    return formData;
  }
  constructor(private http: HttpClient) { }
}
