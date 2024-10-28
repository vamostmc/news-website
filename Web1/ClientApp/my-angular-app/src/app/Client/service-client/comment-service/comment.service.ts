import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Comment } from '../../models/comment';
import { FormGroup, FormsModule } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = 'https://localhost:7233/api/Comment/GetAllComment';
  private PostUrl = 'https://localhost:7233/api/Comment/AddCommentNew';
  private GetIdUrl = 'https://localhost:7233/api/Comment/GetCommentById';
  private DeleteUrl = 'https://localhost:7233/api/Comment/DeleteComment';
  private PutUrl = 'https://localhost:7233/api/Comment/EditCommnent';
  private UpdateStatusUrl = 'https://localhost:7233/api/Comment/UpdateStatus'
  
  //Lấy tất cả danh sách comment
  getAllComment():Observable<Comment[]>{
    return this.http.get<Comment[]>(this.apiUrl);
  } 

  //Lấy mình phần comment đã select
  getIdComment(id: number): Observable<Comment> {
    const url = `${this.GetIdUrl}/${id}`;
    return this.http.get<Comment>(url);
  }

  //Post comment
  PostComment(Data: any): Observable<Comment> {
    return this.http.post<Comment>(this.PostUrl,Data);
  }

  //Put Commment
  PutComment(id: number, Data: any): Observable<Comment> {
    const url = `${this.PutUrl}/${id}`;
    return this.http.put<Comment>(url,Data);
  }

  //Cập nhật status
  StatusComment(id: number,status: boolean): Observable<Comment[]> {
    const url = `${this.UpdateStatusUrl}/${id}`;
    return this.http.put<Comment[]>(url, status);
  }

  //Delete Comment
  DeleteComment(id: number): Observable<any> {
    const url = `${this.DeleteUrl}/${id}`;
    return this.http.delete<any>(url);
  }

  //Chuyển dạng FormGroup Post từ bên user sang dạng FormData
  SendFormPost(postForm: FormGroup): FormData {
    const formData = new FormData();

    formData.append("tintucId", postForm.get("tintucId")?.value);
    formData.append("userId", postForm.get("userId")?.value);
    formData.append("ngayGioBinhLuan", postForm.get("ngayGioBinhLuan")?.value);
    formData.append("noiDung", postForm.get("noiDung")?.value);
    formData.append("trangThai", postForm.get("trangThai")?.value);
    return formData;
  }

  //Chuyển dạng FormGroup Put từ bên user sang FormData
  SendFormEdit(postForm: FormGroup): FormData {
    const formData = new FormData();
    formData.append("binhluanId", postForm.get("binhluanId")?.value);
    formData.append("tintucId", postForm.get("tintucId")?.value);
    formData.append("userId", postForm.get("userId")?.value);
    formData.append("ngayGioBinhLuan", postForm.get("ngayGioBinhLuan")?.value);
    formData.append("noiDung", postForm.get("noiDung")?.value);
    formData.append("trangThai", postForm.get("trangThai")?.value);
    return formData;
  }
  constructor(private http: HttpClient) { }
}
