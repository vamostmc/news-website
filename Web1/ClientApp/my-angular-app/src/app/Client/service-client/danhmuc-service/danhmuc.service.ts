import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Danhmuc } from '../../models/danhmuc';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormBuilder } from '@angular/forms';
import { error } from 'console';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class DanhmucService {
  private apiUrl = 'https://localhost:7233/api/DanhMuc/GetDanhmuc';
  private apiIdUrl = 'https://localhost:7233/api/DanhMuc/GetDanhmuc';
  private postUrl = 'https://localhost:7233/api/DanhMuc/AddDanhMuc';
  private deleteUrl = 'https://localhost:7233/api/DanhMuc/RemoveDanhMuc'
  private putUrl = 'https://localhost:7233/api/DanhMuc/EditDanhMuc';
  private statusUrl = 'https://localhost:7233/api/DanhMuc/EditStatusDanhMuc';

  constructor(private http: HttpClient, private route : Router) { }


  // Lấy tất cả danh mục và số lượng tin tức trong danh mục
  GetDanhmuc(): Observable<Danhmuc[]> {
      return this.http.get<Danhmuc[]>(this.apiUrl);
  }

  // Lấy danh mục Id đã chọn 
  GetDanhMucById(id: number): Observable<Danhmuc> {
    const url = `${this.apiIdUrl}/${id}`;
    return this.http.get<Danhmuc>(url);
  }

  //Thêm Danh mục mới
  PostDanhMuc(newDanhMuc : any): Observable<Danhmuc> {
    return this.http.post<Danhmuc>(this.postUrl,newDanhMuc);
  }

  //Xóa danh mục
  DeleteDanhMuc(id: number): Observable<string> {
    const url = `${this.deleteUrl}/${id}`;
    return this.http.delete<string>(url);
  }

  //Sửa danh mục
  UpdateDanhMuc(id: number, newDanhMuc: any): Observable<Danhmuc> {
    const url = `${this.putUrl}/${id}`;
    return this.http.put<Danhmuc>(url,newDanhMuc);
  }

  //Update trạng thái 
  UpdateStatus(id: number, status: boolean): Observable<Danhmuc[]> {
    const url = `${this.statusUrl}/${id}`;
    return this.http.put<Danhmuc[]>(url,status);
  }

  //Send Form từ User
  SetFormData(FormUser: FormGroup): FormData {
    const formData = new FormData();

    formData.append("tenDanhMuc", FormUser.get("tenDanhMuc")?.value);
    formData.append("trangThai", FormUser.get("trangThai")?.value);  // Chuyển boolean thành string
    formData.append("soLuongTinTuc", FormUser.get("soLuongTinTuc")?.value);  
    return formData;
  }

  SetFormDataNew(FormUser: any): FormData {
    const formData = new FormData();

    Object.keys(FormUser).forEach(key => {
      formData.append(key, FormUser[key]);
    });
    return formData;
  }


  SendDataPost(FormUser: FormGroup) {
    this.PostDanhMuc(this.SetFormDataNew(FormUser)).subscribe({
      next: (response) => {
        console.log('Thêm thành công', response);
        this.route.navigate(['/success']);
      },
      error: (error) => {
        console.error('Lỗi khi thêm danh mục', error);
      },
    });
  }

  SendDataUpdate(id: number, FormUser: FormGroup) {
    this.UpdateDanhMuc(id, this.SetFormData(FormUser)).subscribe({
      next: (response) => {
        console.log('Sửa thành công', response);
        this.route.navigate(['/success']);
      },
      error: (error) => {
        console.error('Lỗi khi sửa danh mục', error);
      },
    });
  }



}
