import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Danhmuc } from '../../models/danhmuc';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormBuilder } from '@angular/forms';
import { error } from 'console';
import { Router } from '@angular/router';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class DanhmucService {

  constructor(private http: HttpClient, private route : Router) { }

  GetDanhmuc(): Observable<Danhmuc[]> {
    return this.http.get<Danhmuc[]>(API_ENDPOINTS.danhMuc.getAll);
  }
  
  // Lấy danh mục theo ID đã chọn
  GetDanhMucById(id: number): Observable<Danhmuc> {
    return this.http.get<Danhmuc>(API_ENDPOINTS.danhMuc.getById(id));
  }
  
  // Thêm danh mục mới
  PostDanhMuc(newDanhMuc: any): Observable<Danhmuc> {
    return this.http.post<Danhmuc>(API_ENDPOINTS.danhMuc.add, newDanhMuc);
  }
  
  // Xóa danh mục
  DeleteDanhMuc(id: number): Observable<string> {
    return this.http.delete<string>(API_ENDPOINTS.danhMuc.delete(id));
  }
  
  // Sửa danh mục
  UpdateDanhMuc(id: number, newDanhMuc: any): Observable<Danhmuc> {
    return this.http.put<Danhmuc>(API_ENDPOINTS.danhMuc.update(id), newDanhMuc);
  }
  
  // Cập nhật trạng thái danh mục
  UpdateStatus(id: number, status: boolean): Observable<Danhmuc[]> {
    return this.http.put<Danhmuc[]>(API_ENDPOINTS.danhMuc.updateStatus(id), status);
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
